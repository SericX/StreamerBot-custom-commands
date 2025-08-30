# RewardLeaderboard
Allows users to search for quotes by string (Streamer.bot v1.0.0+ only)

# How to implement?
You'll need to add it as a C# Code subaction and update the properties in the code. While adding the subaction, you may also need to click "Find Refs" in your C# editor to add a reference to LiteDB. Streamer.Bot found this in my default installation directory.

## topx
By default, set to 3 to output the top 3 redeemers. You can change this to whatever number you like...theoretically. 

## path
This is the location of your Streamer.Bot twitch_data.db file. This is contained in the data folder of your Streamer.Bot installation

## rewardId
Your reward has a unique ID associated with it, you can find out what this is in Streamer.Bot by right-clicking the reward, and selecting 'Copy Reward Id' 

# Example Output
Top 3 check-ins - #1 User1: 3 | #2 User2: 2 | #3 User3: 1