# import pyproj
# import urllib.parse
import json

with open('land_use_and_elevation_data.json') as f:
    content = json.load(f)


land_use_labels = set()
for item in content:
    for key in item.keys():
        for value in item[key]:
            land_use_labels.add(value['landUseLabel'])


for label in land_use_labels:
    print(label)

print(len(land_use_labels))

land_use_labels = list(land_use_labels)
with open('land_use_types.json', 'w') as json_file:
    json.dump(land_use_labels, json_file, indent=4)
