import praw
import json
import random

def reddit():
    with open('Storage/input.json', 'r') as file:
        data = json.load(file)

    reddit = praw.Reddit(client_id="HxYeD-p6A2b4BLCc6PlG7w",
    client_secret="ZcJOWbq5HGYc4TLfNKxPYWGZoXlY1g",
    user_agent="Discord Teabot by /u/Teanquisitor",
    username="Significant_Check591",
    password="vh3P5#LPRFO34bMKVC")
    subreddit = reddit.subreddit(data['Subreddit'])
    top = subreddit.top(limit=256)
    submissions = [submission for submission in top]
    meme = random.choice(submissions)

    with open('Storage/output.json', 'w') as file:
        data['Title'] = meme.title
        data['URL'] = meme.url
        data['Thumbnail'] = meme.thumbnail
        json.dump(data, file)

reddit()