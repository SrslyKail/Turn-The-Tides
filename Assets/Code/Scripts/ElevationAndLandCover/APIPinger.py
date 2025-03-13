import pyproj
import requests
import json
from bs4 import BeautifulSoup
import numpy as np
import urllib.parse

#Weird map projection stuff
proj_3005 = pyproj.CRS("EPSG:3005")
proj_wgs84 = pyproj.CRS("EPSG:4326")
transformer_to_wgs84 = pyproj.Transformer.from_crs(proj_3005, proj_wgs84, always_xy=True)
transformer_to_3005 = pyproj.Transformer.from_crs(proj_wgs84, proj_3005, always_xy=True)

#These are fairly arbitrary based on WMS values, don't mess with them unless you know what you're doing
min_x = 1196344.44723984622396529 # Lower left corner X (in EPSG:3005)
min_y = 446362.68979797093197703  # Lower left corner Y (in EPSG:3005)
max_x = 1318476.11390651320107281  # Upper right corner X (in EPSG:3005)
max_y = 511979.35646463779266924  # Upper right corner Y (in EPSG:3005)

map_width = 2308
map_height = 1240


# Start is set to 49.005 because 49 causes errors
start_lat = 49.005
end_lat = 49.5
start_lon = -123.3
end_lon = -121.675

#Set num_lat_steps to the number of columns you want
#Set num_lon_steps to the number of rows you want
num_lat_steps = 168
num_lon_steps = 75


def lat_lon_to_ij(lat, lon):
    """
    Gets the I and J parameters for the given latitude and longitude
    :param lat: latitude
    :param lon: longitude
    :return: I and J pixel values for land use lookup
    """
    x_3005, y_3005 = transformer_to_3005.transform(lon, lat)

    rel_x = (x_3005 - min_x) / (max_x - min_x)
    rel_y = (y_3005 - min_y) / (max_y - min_y)

    i = int(rel_x * map_width)
    j = int((1 - rel_y) * map_height)

    i = min(max(i, 0), map_width - 1)
    j = min(max(j, 0), map_height - 1)

    return i, j

# Function to get the land use label from the WMS API using I/J coordinates
def get_land_use_label(i, j):
    """
    Gets the land use label for the given I and J values
    :param i: I's pixel value
    :param j: J's pixel value
    :return: Label for land use
    """

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
            print(f"Land Use Label: {label_value}")
            return label_value
        else:
            return "Unknown"
    else:
        return f"Error: {response.status_code}"


def get_elevation(lat, lon):
    """
    Gets the elevation for the given latitude and longitude
    :param lat: latitude
    :param lon: longitude
    :return: altitude
    """

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




latitudes = np.linspace(start_lat, end_lat, num_lat_steps)
longitudes = np.linspace(start_lon, end_lon, num_lon_steps)

result_data = []

for lat_index, lat in enumerate(latitudes):
    lat_data = []
    for lon_index, lon in enumerate(longitudes):
        i, j = lat_lon_to_ij(lat, lon)

        print(f"Requesting data for Latitude: {lat}, Longitude: {lon}")
        land_use_label = get_land_use_label(i, j)

        elevation = get_elevation(lat, lon)
        print(f"Elevation: {elevation}")

        lat_data.append({
            "latitude": lat,
            "longitude": lon,
            "landUseLabel": land_use_label,
            "elevation": elevation
        })

    result_data.append({
        lat_index: lat_data
    })

with open('land_use_and_elevation_data.json', 'w') as json_file:
    json.dump(result_data, json_file, indent=4)

print("Test data saved to 'land_use_and_elevation_data.json'")
