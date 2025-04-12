# Turn-The-Tides

## Custom Maps
### JSON Format:
JSON files for custom maps must adhere to the following format:
```JSON
[
    {
        "0": [
            {
                "latitude": 0,
                "longitude": 0,
                "landUseLabel": "Salt Water",
                "elevation": 0.0
            },
            {
                "latitude": 0,
                "longitude": 1,
                "landUseLabel": "Salt Water",
                "elevation": 0.0
            }
        ]
    },
    {
        "1": [
            {
                "latitude": 1,
                "longitude": 0,
                "landUseLabel": "Salt Water",
                "elevation": 0.0
            },
            {
                "latitude": 1,
                "longitude": 1,
                "landUseLabel": "Salt Water",
                "elevation": 0.0
            }
        ]
    }
]
```
The outer array represents the map in its entirety, and is composed of "numbered" (with strings) arrays which represent each row of hex tiles on the hex grid map.

While row and column count aren't explicitly limited, performance may decrease at higher map sizes beyond 50,000 tiles (or fewer depending on your hardware specs).

Latitude and longitude are not functional parts of defining the hex tile location, but can be used to represent their real-world location.

Elevation is a float which determines the height of each hex tile in meters.

(Note: the scale represented in Turn The Tides is exaggerated at the low end with a taper as the height increases.)

Currently implemented landUseLabels are as follows:
<ul>
<li>
Barren
<li>
Ocean
<li>
River
<li>
Lake
<li>
Swamp
<li>
Land
<li>
Forest
<li>
Urban
<li>
Farm
<li>
Snow
<li>
Rural
</ul>

Unsupported labels will default to the "Barren" tile type.

### Loading a Custom Map:
Once you have a valid custom map JSON, 
<ol>
<li> Open Turn The Tides.
<li> Click "Load Level" from the main menu.
<li> Click "Select File".
<li> Navigate to and open your custom JSON.
<li> Adjust Map Scale (1 will use every defined tile, 2 will use every second, etc.)
<li> Adjust Flood Amount (How many centimetes sea levels will rise per turn.)
<li> Click "Create Map" and wait for confirmation that the map is loaded.
<li> Click "Back".
<li> Click "Start" to load your custom map!