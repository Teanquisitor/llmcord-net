import json
from googletrans import Translator

def translate():
    with open('Storage/input.json', 'r') as file:
        data = json.load(file)

    translation = Translator().translate(data['Text'], dest=data['Language']).text

    with open('Storage/output.json', 'w') as file:
        data['Text'] = translation
        json.dump(data, file)

translate()