using System;
using System.Collections.Generic;

// v0.0.1

public class CPHInline
{
    //Make a list of quotes
    List<QuoteData> quotes = new();
    public bool Execute()
    {
        //Populate the quotes list
        FetchQuotes();
        //Set up a null quote object
        QuoteData quote;
        //Read the input
        string input0 = args.ContainsKey("inputEscaped0") ? args["inputEscaped0"].ToString() : "";
        //Check if it's a number
        bool isNumeric = int.TryParse(input0, out int n);
        //If numeric, assume we're looking for an Id
        if (isNumeric)
        {
            //If we don't find something by ID, let's give a string search a go
            quote = FindQuoteById(n) ?? FindQuoteByString(input0.ToUpper());
        }
        else
        {
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
        }

        //Print that quote!
        PrintQuote(quote);
        return true;
    }

    //Find a quote by quote text
    private QuoteData FindQuoteByString(string searchStr)
    {
        //Iterate over all the quotes
        foreach (QuoteData quote in quotes)
        {
            //If the quote text contains the search string, return it
            if (quote.Quote.ToUpper().Contains(searchStr))
            {
                return quote;
            }
        }

        //If we don't find a match, return null
        return null;
    }

    //Find a quote by Id
    private QuoteData FindQuoteById(int id)
    {
        //Iterate over all the quotes
        foreach (QuoteData quote in quotes)
        {
            //If the quote ID matches the provided ID return it
            if (quote.Id == id)
            {
                return quote;
            }
        }

        //If no match, return null
        return null;
    }

    /*
	Fetch all the quotes and put them in a list - based on code from Streamerbot support Discord member Rondhi
	We've got to do this because:
	- There's no continuous index 
	- There's no function to get the max index (just the count of quotes)
	- Trying to retrieve a quote that has been deleted breaks shit 
	I may be dentge, it's late
	*/
    private void FetchQuotes()
    {
        int maxQuoteNumber = CPH.GetQuoteCount();
        //We're gonna assume we've deleted fewer than 1000 quotes
        maxQuoteNumber += 100;
        //Iterate over the range of maximum quote numbers and retrieve each quote
        for (int i = 1; i <= maxQuoteNumber; i++)
        {
            try
            {
                //Retrieve the quote at index 'i'
                QuoteData quote = CPH.GetQuote(i);
                //If no quote is found, skip to the next index
                if (quote == null)
                {
                    continue;
                }

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
            //List of emotes to use for quotes not found
            string[] quoteNotFoundEmotes =
            {
                "emote1",
                "emote2"
            };
            //Message sent when a quote is not found, matches format: Quote not found <Emote from quoteNotFoundEmotes list>
            CPH.SendMessage($"Quote not found {quoteNotFoundEmotes[new Random().Next(0, quoteNotFoundEmotes.Length)]}");
            return;
        }

        //List of emotes to use for quotes found
        string[] quoteFoundEmotes =
        {
            "emote1",
            "emote2"
        };
        //Message sent when a quote is found, matches format: "Quote" ( #QuoteID ) ( Game ) <Emote from quoteFoundEmotes list>
        CPH.SendMessage($"\"{quote.Quote}\" ( #{quote.Id} ) ( {quote.GameName} ) {quoteFoundEmotes[new Random().Next(0, quoteFoundEmotes.Length)]}");
    }
}
