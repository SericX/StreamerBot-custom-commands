using System;
using System.Collections.Generic;

/*
------------------------
v1.0.0
QuoteSearch allows users to search for quotes by string
Only works with Streamer.bot v1.0.0+
Add to default subactions to find quote by string
------------------------
*/
public class CPHInline
{
    /* ===Start of user definiable values===*/
    //Default behaviour for quotesearch returns the first item if multiple quotes are found when searching by string. If you want a random quote instead set this to 'true'.
    private const bool _quoteSearchRandomQuoteReturn = false;
    //If we hit a run of 5 unfetchable quotes, assume we've reached the end of the quote list
    //Note: You may want to increase this if you delete lots of quotes and run into problems
    private const int _maxConsecutiveFailures = 5;
    //Streamer.bot stores quotes with a permenant index, but doesn't expose how many have been deleted so we need to assume an amount are missing
    //If you delete a lot of quotes, you may need to increase this number
    private const int _quoteIndexSurplus = 100;
    //List of emotes to use for quotes found
    private string[] _quoteFoundEmotes =
    {
        "emote1",
        "emote2"
    };
    //List of emotes to use for quotes not found
    private string[] _quoteNotFoundEmotes =
    {
        "emote1",
        "emote2"
    };
    /*===End of user definiable values===*/
    //Make a list of quotes
    private List<QuoteData> quotes = new();
    public bool Execute()
    {
        //Populate the quotes list
        FetchQuotes();
        QuoteData quote;
        //Read the input
        string input0 = args.ContainsKey("inputEscaped0") ? args["inputEscaped0"].ToString() : "";
        //Check if the input is empty or whitespace
        if (input0.Trim() != "")
        {
            //If input is string, assume search by string
            quote = FindQuoteByString(input0.ToUpper());
        }
        else
        {
            //If input is blank, assume random quote
            quote = quotes[new Random().Next(0, quotes.Count)];
        }

        //Print that quote!
        PrintQuote(quote);
        return true;
    }

    //Find a quote by quote text
    private QuoteData FindQuoteByString(string searchStr)
    {
        List<QuoteData> foundQuotes = new();
        //Iterate over all the quotes
        foreach (QuoteData quote in quotes)
        {
            //If the quote text contains the search string, return it
            if (quote.Quote.ToUpper().Contains(searchStr))
            {
                //If random return isn't enabled, return the first quote
                if (!_quoteSearchRandomQuoteReturn)
                {
                    return quote;
                }

                //If random return is enabled, add the quote to a list
                foundQuotes.Add(quote);
            }
        }

        //Return a random found quote if we found any
        if (foundQuotes.Count > 0)
        {
            return foundQuotes[new Random().Next(0, foundQuotes.Count)];
        }

        //If we don't find a match, return null
        return null;
    }

    /*
	Fetch all the quotes and put them in a list.
	We've got to do this because:
	- There's no continuous index (unless you reindex your quotes)
	- There's no function to get the max index (just the count of quotes [unless you reindex your quotes])
	- Trying to retrieve a quote that has been deleted breaks shit (unless you reindex your quotes)
	I may be dentge, it's late
	*/
    private void FetchQuotes()
    {
        int maxQuoteNumber = CPH.GetQuoteCount();
        int consecutiveFailures = 0;
        //Iterate over the range of maximum quote numbers and retrieve each quote
        for (int i = 1; i <= maxQuoteNumber + _quoteIndexSurplus; i++)
        {
            try
            {
                //Retrieve the quote at index 'i'
                QuoteData quote = CPH.GetQuote(i);
                //If no quote is found, skip to the next index
                if (quote == null)
                {
                    consecutiveFailures++;
                    if (consecutiveFailures >= _maxConsecutiveFailures)
                    {
                        break;
                    }

                    continue;
                }

                consecutiveFailures = 0;
                //If a quote is found, add it to the quotes list
                quotes.Add(quote);
            }
            catch
            {
                continue;
            }
        }
    }

    //Formats the quote text and adds a fun emote on the end
    private void PrintQuote(QuoteData quote)
    {
        //If the quote is null, quote not found
        if (quote == null)
        {
            //Message sent when a quote is not found, matches format: Quote not found <Emote from quoteNotFoundEmotes list>
            CPH.SendMessage($"Quote not found {_quoteNotFoundEmotes[new Random().Next(0, _quoteNotFoundEmotes.Length)]}");
            return;
        }

        //Message sent when a quote is found, matches format: "Quote" ( #QuoteID ) ( Game ) <Emote from quoteFoundEmotes list>
        CPH.SendMessage($"\"{quote.Quote}\" ( #{quote.Id} ) ( {quote.GameName} ) {_quoteFoundEmotes[new Random().Next(0, _quoteFoundEmotes.Length)]}");
    }
}