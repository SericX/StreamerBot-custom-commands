using System;
using LiteDB;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

public class RewardRedeemer
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public long Count { get; set; }
}

public class CPHInline
{
    //How many places do you you want to display on your leaderboard?
    private const int topx = 3;
    //Locaiton of your twitch_data.db
    private const string path = @"Your-streamer.bot-installation-path\StreamerBot\data\twitch_data.db";
    //What is your rewardId?
    private string rewardId = "Your persistent user tracked reward Id";
    public bool Execute()
    {
        List<RewardRedeemer> leaderboard = new List<RewardRedeemer>();
        //Important that we close the connection quickly, or it can crash Streamer.Bot
        using (LiteDatabase liteDatabase = new LiteDatabase(path))
        {
            //Fetch the reward redemptions list and order by redemptions descending
            leaderboard = liteDatabase.GetCollection("rewardRedemptionUserCounts")
            .Find(Query.EQ("rewardid", rewardId))
            .Select(x => new RewardRedeemer { UserId = x["userid"].AsString, Count = x["count"].AsInt32 })
            .OrderByDescending(x => x.Count)
            .Take(topx)
            .ToList();
        }

        //Fetch and populate the UserNames for the leaderboard
        leaderboard = HydrateLeaderboard(leaderboard);
        //Put the string together
        string msg = ConstructLeaderboardString(leaderboard);
        //Output the leaderboard string
        CPH.SendMessage(msg);
        return true;
    }

    private List<RewardRedeemer> HydrateLeaderboard(List<RewardRedeemer> leaderboardToHydrate)
    {
        //Populate the username property
        foreach (RewardRedeemer item in leaderboardToHydrate)
        {
            TwitchUserInfo tui = CPH.TwitchGetUserInfoById(item.UserId);
            item.UserName = tui.UserName;
        }

        return leaderboardToHydrate;
    }

    private string ConstructLeaderboardString(List<RewardRedeemer> leaderboardToOutput)
    {
        //Output the scoreboard
        string msg = "";
        int place = 1;
        foreach (RewardRedeemer redeemer in leaderboardToOutput)
        {
            if (place > 1)
            {
                msg += $" | #{place} {redeemer.UserName}: {redeemer.Count}";
            }
            else
            {
                msg += $"Top {topx} check-ins - #{place} {redeemer.UserName}: {redeemer.Count}";
            }

            place++;
        }

        return msg;
    }
}