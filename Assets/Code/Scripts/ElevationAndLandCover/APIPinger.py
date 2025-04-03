import pyproj
import requests
import json
from bs4 import BeautifulSoup
import numpy as np
import urllib.parse
from geopy.distance import geodesic
import traceback
import concurrent.futures

"""Map translation stuff"""
proj_3005 = pyproj.CRS("EPSG:3005")
proj_wgs84 = pyproj.CRS("EPSG:4326")
transformer_to_wgs84 = pyproj.Transformer.from_crs(proj_3005, proj_wgs84, always_xy=True)
transformer_to_3005 = pyproj.Transformer.from_crs(proj_wgs84, proj_3005, always_xy=True)

"""Land use map image bounding values"""
min_x = 1196344.44723984622396529
min_y = 446362.68979797093197703
max_x = 1318476.11390651320107281
max_y = 511979.35646463779266924
map_width = 2308
map_height = 1240

"""API pinging lat/long bounding values"""
start_lat = 49.005
end_lat = 49.5
start_lon = -123.3
end_lon = -121.3

"""Set the scale for pinging latitudinally, the 500 is 500 meters out of the total meter width """
lat_step_size = 500 / 111320


def get_step_sizes(lat):
    """Calculates the appropriate step sizes to move 500m at the given latitude."""
    origin = (lat, start_lon)
    point = (lat, start_lon + 1)
    longitude_step_size = geodesic(origin, point).meters
    longitude_step_size = 500 / longitude_step_size
    return longitude_step_size


def lat_lon_to_ij(lat, lon):
    """Convert lat/lon to pixel I, J on the map."""
    x_3005, y_3005 = transformer_to_3005.transform(lon, lat)
    rel_x = (x_3005 - min_x) / (max_x - min_x)
    rel_y = (y_3005 - min_y) / (max_y - min_y)
    i = int(rel_x * map_width)
    j = int((1 - rel_y) * map_height)
    i = min(max(i, 0), map_width - 1)
    j = min(max(j, 0), map_height - 1)
    return i, j


def get_land_use_label(i, j):
    """Fetches the land use label for the given pixel I, J."""
    url = 'https://openmaps.gov.bc.ca/geo/pub/WHSE_BASEMAPPING.BTM_PRESENT_LAND_USE_V1_SVW/ows'
    params = {
        'SERVICE': 'WMS',
        'VERSION': '1.3.0',
        'REQUEST': 'GetFeatureInfo',
        'BBOX': f"{min_x},{min_y},{max_x},{max_y}",
        'CRS': 'EPSG:3005',
        'WIDTH': str(map_width),
        'HEIGHT': str(map_height),
        'LAYERS': 'pub:WHSE_BASEMAPPING.BTM_PRESENT_LAND_USE_V1_SVW',
        'STYLES': '',
        'FORMAT': 'image/png',
        'QUERY_LAYERS': 'pub:WHSE_BASEMAPPING.BTM_PRESENT_LAND_USE_V1_SVW',
        'INFO_FORMAT': 'text/html',
        'I': str(i),
        'J': str(j),
        'FEATURE_COUNT': '10'
    }
    wms_url = f"{url}?{urllib.parse.urlencode(params)}"
    response = requests.get(wms_url)
    if response.status_code == 200:
        soup = BeautifulSoup(response.text, 'html.parser')
        label_row = soup.find('td', string='PRESENT_LAND_USE_LABEL')
        if label_row:
            label_value = label_row.find_next('td').text.strip()
            return label_value
        else:
            return "Unknown"
    else:
        return f"Error: {response.status_code}"


def get_elevation(lat, lon):
    """Fetches the elevation for the given lat/lon."""
    url = f'http://geogratis.gc.ca/services/elevation/cdem/altitude?lat={lat}&lon={lon}'
    response = requests.get(url)
    if response.status_code == 200:
        try:
            data = response.json()
            altitude = data.get('altitude', "Unknown")
            return altitude
        except ValueError:
            return "Error parsing JSON response"
    else:
        return f"Error: {response.status_code}"


def process_lat_lon(lat, lon):
    """Process one lat/lon pair to get land use and elevation."""
    try:
        i, j = lat_lon_to_ij(lat, lon)
        land_use_label = get_land_use_label(i, j)
        elevation = get_elevation(lat, lon)
        return {
            "latitude": lat,
            "longitude": lon,
            "landUseLabel": land_use_label,
            "elevation": elevation
        }
    except Exception as e:
        print(f"Error processing Latitude: {lat}, Longitude: {lon}. Error: {str(e)}")
        traceback.print_exc()
        return None


latitudes = np.arange(start_lat, end_lat, lat_step_size)
lon_step = get_step_sizes(start_lat)
longitudes = np.arange(start_lon, end_lon, lon_step)

result_data = []

"""Concurrency shenanigans (cuts the pinging time down from 6~ hours to 15~ minutes!"""
with concurrent.futures.ThreadPoolExecutor(max_workers=75) as executor:
    for lat_index, lat in enumerate(latitudes):
        futures = []
        for lon in longitudes:
            futures.append(executor.submit(process_lat_lon, lat, lon))

        row_data = []
        for future in concurrent.futures.as_completed(futures):
            result = future.result()
            if result:
                row_data.append(result)

        row_data.sort(key=lambda x: x['longitude'])

        result_data.append({
            str(lat_index): row_data
        })

with open('land_use_and_elevation_data.json', 'w') as json_file:
    json.dump(result_data, json_file, indent=4)

print("Data saved to 'land_use_and_elevation_data.json'")
