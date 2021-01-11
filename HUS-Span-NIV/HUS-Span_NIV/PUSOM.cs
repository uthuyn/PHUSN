using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
public class PUSOM
{
    public double numberOfPatterns = 0;
    /// <summary>
    /// if true, debugging information will be shown in the console </summary>
    readonly bool DEBUG = false;

    #region "Declaration Variable"
    int sequenceCount = 0;
    /// <summary>
    /// the time the algorithm started </summary>
    public long startTimestamp = 0;
    /// <summary>
    /// the time the algorithm terminated </summary>
    public long endTimestamp = 0;
    /// <summary>
    /// record memory usage the algorithm terminated </summary>
    public Process currentProc;
    /// <summary>
    /// the number of HUSP generated </summary>
    public int patternCount = 0;

    /// <summary>
    /// writer to write the output file * </summary>
    //StreamWriter writer = null;

    /// <summary>
    /// buffer for storing the current pattern that is mined when performing mining
    /// the idea is to always reuse the same buffer to reduce memory usage. *
    /// </summary>
    readonly int BUFFERS_SIZE = 2000;
    private int[] patternBuffer = null;

    /// <summary>
    /// if true, save result to file in a format that is easier to read by humans * </summary>
    //readonly bool SAVE_RESULT_EASIER_TO_READ_FORMAT = true;

    /// <summary>
    /// the minUtility threshold * </summary>
    float minUtility = 0;

    /// <summary>
    /// max pattern length * </summary>
    int maxPatternLength = int.MaxValue;

    /// <summary>
    /// the input file path * </summary>
    string input;
    /// <summary>
    /// database file
    /// </summary>
    IList<QMatrixHUSSpan> database;

    // create a map to store the SWU of each item
    // key: item  value: the swu of the item
    Dictionary<int, float> mapItemToSWU;
    //List to save High utility pattern 
    public Dictionary<int[], float> highUtilitySet = new Dictionary<int[], float>();
    #endregion

    #region "Load data with internal and external"
    Dictionary<int, float> externalData;
    public IList<QMatrixHUSSpan> loadDataWithInternalExternal(string external, string input, float minUtility)
    {
        //Read External value
        externalData = new Dictionary<int, float>();
        System.IO.StreamReader externalInput = null;
        string line;
        // prepare the object for reading the file
        //try
        //{
        externalInput = new System.IO.StreamReader(new System.IO.FileStream(external, System.IO.FileMode.Open, System.IO.FileAccess.Read));
        // for each line (transaction) until the end of file
        while ((line = externalInput.ReadLine()) != null)
        {
            int position_Colons = line.IndexOf(':');
            string key = line.Substring(0, position_Colons);
            int item = int.Parse(key);
            string value = line.Substring(position_Colons + 1, line.Length - (position_Colons + 1));
            float price = float.Parse(value);
            externalData.Add(item, price);
        }
        //}
        //catch (Exception e)
        //{
        //    // catches exception if error while reading the input file
        //    Console.WriteLine("No data path found");
        //}
        //finally
        //{
        //    if (externalInput != null)
        //    {
        //        externalInput.Close();
        //    }
        //}
        // input path
        this.input = input;

        // initialize the buffer for storing the current itemset
        patternBuffer = new int[BUFFERS_SIZE];

        // save the minimum utility threshold
        this.minUtility = minUtility;

        // create a map to store the SWU of each item
        // key: item  value: the swu of the item
        mapItemToSWU = new Dictionary<int, float>();

        // ==========  FIRST DATABASE SCAN TO IDENTIFY PROMISING ITEMS =========
        // We scan the database a first time to calculate the SWU of each item.
        System.IO.StreamReader myInput = null;
        string thisLine;
        //try
        //{
        // prepare the object for reading the file
        myInput = new System.IO.StreamReader(new System.IO.FileStream(input, System.IO.FileMode.Open, System.IO.FileAccess.Read));
        // for each line (transaction) until the end of file
        while ((thisLine = myInput.ReadLine()) != null)
        {
            // if the line is a comment, is  empty or is a kind of metadata, skip it
            if (thisLine.Length == 0 || thisLine[0] == '#' || thisLine[0] == '%' || thisLine[0] == '@')
            {
                continue;
            }
            // split the transaction according to the " " separator
            string[] tokens = thisLine.Split(" ", true);

            // get the sequence utility (the last token on the line)
            string sequenceUtilityString = tokens[tokens.Length - 1];
            int positionColons = sequenceUtilityString.IndexOf(':');
            float sequenceUtility = float.Parse(sequenceUtilityString.Substring(positionColons + 1));
            // Then read each token from this sequence (except the last three tokens
            // which are -1 -2 and the sequence utility)
            for (int i = 0; i < tokens.Length - 3; i++)
            {
                string currentToken = tokens[i];
                // if the current token is not -1 
                if (currentToken.Length != 0 && currentToken[0] != '-')
                {
                    // find the left brack
                    int positionLeftBracketString = currentToken.IndexOf('[');
                    // get the item
                    string itemString = currentToken.Substring(0, positionLeftBracketString);
                    int item = int.Parse(itemString);

                    // get the current SWU of that item
                    float swu = 0;
                    if (mapItemToSWU.ContainsKey(item))
                    {
                        swu = mapItemToSWU[item];
                    }
                    // add the utility of sequence utility to the swu of this item
                    if (swu == 0)
                        swu = sequenceUtility;
                    else
                        swu += sequenceUtility;
                    mapItemToSWU[item] = swu;
                }
            }

            // increase sequence count
            sequenceCount++;
        }
        #region "Debug"
        // If we are in debug mode, we will show the number of distinct items in the database,
        // the number of sequences and the SWU of each item
        if (DEBUG)
        {
            Console.WriteLine("INITIAL ITEM COUNT " + mapItemToSWU.Count);
            Console.WriteLine("SEQUENCE COUNT = " + sequenceCount);
            Console.WriteLine("INITIAL SWU OF ITEMS");
            foreach (KeyValuePair<int, float> entry in mapItemToSWU.SetOfKeyValuePairs())
            {
                Console.WriteLine("Item: " + entry.Key + " swu: " + entry.Value);
            }
        }
        #endregion
        //================  SECOND DATABASE SCAN ===================
        // Read the database again to create QMatrix for each sequence
        database = new List<QMatrixHUSSpan>();
        //try
        //{
        // prepare the object for reading the file
        myInput = new System.IO.StreamReader(new System.IO.FileStream(input, System.IO.FileMode.Open, System.IO.FileAccess.Read));

        // We will read each sequence in buffers.
        // The first buffer will store the items of a sequence and the -1 between them)
        int[] itemBuffer = new int[BUFFERS_SIZE];
        // The second buffer will store the utility of items in a sequence and the -1 between them)
        float[] utilityBuffer = new float[BUFFERS_SIZE];
        // The following variable will contain the length of the data stored in the two previous buffer
        int itemBufferLength;
        // Finally, we create another buffer for storing the items from a sequence without
        // the -1. This is just used so that we can collect the list of items in that sequence
        // efficiently. We will use this information later to create the number of rows in the
        // QMatrix for that sequence.
        int[] itemsSequenceBuffer = new int[BUFFERS_SIZE];
        // The following variable will contain the length of the data stored in the previous buffer
        int itemsLength;
        //The following variable will store the sequence ID Clark Dinh
        int seqID = 0;
        // for each line (transaction) until the end of file
        while ((thisLine = myInput.ReadLine()) != null)
        {
            // if the line is  a comment, is  empty or is a kind of metadata
            if (thisLine.Length == 0 || thisLine[0] == '#' || thisLine[0] == '%' || thisLine[0] == '@')
            {
                continue;
            }

            // We reset the two following buffer length to zero because
            // we are reading a new sequence.
            itemBufferLength = 0;
            itemsLength = 0;
            // split the sequence according to the " " separator
            string[] tokens = thisLine.Split(" ", true);

            // get the sequence utility (the last token on the line)
            string sequenceUtilityString = tokens[tokens.Length - 1];
            int positionColons = sequenceUtilityString.IndexOf(':');
            float sequenceUtility = float.Parse(sequenceUtilityString.Substring(positionColons + 1));

            // This variable will count the number of itemsets
            int nbItemsets = 1;
            // This variable will be used to remember if an itemset contains at least a promising item
            // (otherwise, the itemset will be empty).
            bool currentItemsetHasAPromisingItem = false;

            // Copy the current sequence in the sequence buffer.
            // For each token on the line except the last three tokens
            // (the -1 -2 and sequence utility).
            for (int i = 0; i < tokens.Length - 3; i++)
            {
                string currentToken = tokens[i];

                // if empty, continue to next token
                if (currentToken.Length == 0)
                {
                    continue;
                }

                // if the current token is -1
                if (currentToken.Equals("-1"))
                {
                    // It means that it is the end of an itemset.
                    // So we check if there was a promising item in that itemset
                    if (currentItemsetHasAPromisingItem)
                    {
                        // If yes, then we keep the -1, because
                        // that itemset will not be empty.

                        // We store the -1 in the respective buffers 
                        itemBuffer[itemBufferLength] = -1;
                        utilityBuffer[itemBufferLength] = -1;
                        // We increase the length of the data stored in the buffers
                        itemBufferLength++;

                        // we update the number of itemsets in that sequence that are not empty
                        nbItemsets++;
                        // we reset the following variable for the next itemset that 
                        // we will read after this one (if there is one)
                        currentItemsetHasAPromisingItem = false;
                    }
                }
                else
                {
                    // if  the current token is an item
                    //  We will extract the item from the string:
                    int positionLeftBracketString = currentToken.IndexOf('[');
                    int positionRightBracketString = currentToken.IndexOf(']');
                    string itemString = currentToken.Substring(0, positionLeftBracketString);
                    int item = int.Parse(itemString);
                    // We also extract the utility from the string:
                    string internalString = currentToken.Substring(positionLeftBracketString + 1, positionRightBracketString - (positionLeftBracketString + 1));
                    float itemUtility = int.Parse(internalString) * externalData[item];

                    // if the item is promising (its SWU >= minutility), then
                    // we keep it in the sequence
                    if (mapItemToSWU[item] >= minUtility)
                    {
                        // We remember that this itemset contains a promising item
                        currentItemsetHasAPromisingItem = true;

                        // We store the item and its utility in the buffers
                        // for temporarily storing the sequence
                        itemBuffer[itemBufferLength] = item;
                        utilityBuffer[itemBufferLength] = itemUtility;
                        itemBufferLength++;

                        // We also put this item in the buffer for all items of this sequence
                        itemsSequenceBuffer[itemsLength++] = item;
                    }
                    else
                    {
                        // if the item is not promising, we subtract its utility 
                        // from the sequence utility, and we do not add it to the buffers
                        // because this item will not be part of a high utility sequential pattern.
                        //TAI MODIFIES FOR NEGATIVE ITEMS
                        if (externalData[item] > 0)
                            sequenceUtility -= itemUtility;
                    }
                }
            }

            // If the sequence utility is now zero, which means that the sequence
            // is empty after removing unpromising items, we don't keep it
            if (sequenceUtility == 0)
            {
                sequenceCount--;
                continue;
            }
            #region "Debug"
            // If we are in debug mode,  
            if (DEBUG)
            {
                // We will show the original sequence
                Console.WriteLine("SEQUENCE BEFORE REMOVING UNPROMISING ITEMS:\n");
                Console.WriteLine(" " + thisLine);
                // We will show the sequence after removing unpromising items
                Console.Write("SEQUENCE AFTER REMOVING UNPROMISING ITEMS:\n ");
                for (int i = 0; i < itemBufferLength; i++)
                {
                    Console.Write(itemBuffer[i] + "[" + utilityBuffer[i] + "] ");
                }
                // And we will thow the sequence utility after removing the unpromising items.
                Console.WriteLine("NEW SEQUENCE UTILITY " + sequenceUtility);
            }
            #endregion
            // Now, we sort the buffer for storing all items from the current sequence
            // in alphabetical order
            Array.Sort(itemsSequenceBuffer, 0, itemsLength);
            // but an item may appear multiple times in that buffer so we will
            // loop over the buffer to remove duplicates
            // This variable remember the last insertion position in the buffer:
            int newItemsPos = 0;
            // This variable remember the last item read in that buffer
            int lastItemSeen = -999;
            // for each position in that buffer
            for (int i = 0; i < itemsLength; i++)
            {
                // get the item
                int item = itemsSequenceBuffer[i];
                // if the item was not seen previously
                if (item != lastItemSeen)
                {
                    // we copy it at the current insertion position
                    itemsSequenceBuffer[newItemsPos++] = item;
                    // we remember this item as the last seen item
                    lastItemSeen = item;
                }
            }
            #region "Debug"
            // If we are in debugging mode
            if (DEBUG)
            {
                // We will print the list of promising items from the sequence,
                // sorted in alphabetical order:
                Console.Write("LIST OF PROMISING ITEMS IN THAT SEQUENCE:\n ");
                for (int i = 0; i < newItemsPos; i++)
                {
                    Console.Write(itemsSequenceBuffer[i] + " ");
                }
                Console.WriteLine();
            }
            #endregion
            // Now we count the number of items in that sequence
            int nbItems = newItemsPos;

            // And we will create the Qmatrix for that sequence
            QMatrixHUSSpan matrix = new QMatrixHUSSpan(nbItems, --nbItemsets, itemsSequenceBuffer, newItemsPos, sequenceUtility);
            matrix.SeqID = ++seqID;  // New Modify by Clark Dinh
            matrix.NbItemsets = nbItemsets;
            // We add the QMatrix to the initial sequence database.
            database.Add(matrix);

            // Next we will fill the matrix column by column
            // This variable will represent the position in the sequence
            int posBuffer = 0;
            // for each itemset (column)
            for (int itemset = 0; itemset < nbItemsets; itemset++)
            {
                // This variable represent the position in the list of items in the QMatrix
                int posNames = 0;
                // While we did not reach the end of the sequence
                while (posBuffer < itemBufferLength)
                {
                    // Get the item at the current position in the sequence
                    int item = itemBuffer[posBuffer];
                    // if it is an itemset separator, we move to next position in the sequence
                    if (item == -1)
                    {
                        //TAI MODIFIES FOR NEGATIVE ITEMS
                        if (posNames < matrix.ItemNames.Length)
                        {
                            item = matrix.ItemNames[posNames];
                            matrix.registerItem(posNames, itemset, 0, sequenceUtility);
                            posNames++;
                        }
                        else
                        {
                            posBuffer++;
                            break;
                        }
                    }
                    // else if it is the item that correspond to the next row in the matrix
                    else if (item == matrix.ItemNames[posNames])
                    {
                        // calculate the utility for this item/itemset cell in the matrix
                        float utility = utilityBuffer[posBuffer];
                        // We update the reamining utility by subtracting the utility of the
                        // current item/itemset
                        //TAI MODIFIES FOR NEGATIVE ITEMS
                        if (externalData[item] > 0)
                            sequenceUtility -= utility;
                        // update the cell in the matrix
                        matrix.registerItem(posNames, itemset, utility, sequenceUtility);
                        // move to the next item in the matrix and in the sequence
                        posNames++;
                        posBuffer++;
                    }
                    else if (item > matrix.ItemNames[posNames])
                    {
                        // if the next item in the sequence is larger than the current row in the matrix
                        // it means that the item do not appear in that itemset, so we put a utility of 0
                        // for that item and move to the next row in the matrix.
                        matrix.registerItem(posNames, itemset, 0, sequenceUtility);
                        posNames++;
                    }
                    else
                    {
                        // Otherwise, we put a utility of 0 for the current row in the matrix and move
                        // to the next item in the sequence
                        matrix.registerItem(posNames, itemset, 0, sequenceUtility);
                        posBuffer++;
                    }
                }
            }

            // if in debug mode, we print the q-matrix that we have just built
            if (DEBUG)
            {
                Console.WriteLine(matrix.ToString());
                Console.WriteLine();
            }
        }
        //}
        //catch (Exception e)
        //{
        //    // catches exception if error while reading the input file
        //    Console.WriteLine("No data path found");
        //    //Console.Write(e.StackTrace);
        //}
        //finally
        //{
        //    if (myInput != null)
        //    {
        //        myInput.Close();
        //    }
        //}
        Console.Write(sequenceCount.ToString());
        return database;
    }
    #endregion

    string arrayToString(int[] a)
    {
        string s = "";
        for (int i = 0; i < a.Length; i++)
            s += a[i].ToString();
        return s;
    }
    #region "PUSOM algorithm"
    //The method for creating the utility chain of 1-length sequence
    private void husspanFirstTimeForPUSOM(int[] prefix, int prefixLength, IList<QMatrixHUSSpan> database)
    {
        // For the first call to HUSSPan, we only need to check I-Extension
        // =======================  I-Extension  ===========================/  
        // For each item 
        //startTimestamp = DateTimeHelperClass.CurrentUnixTimeMillis();
        IDictionary<int, float> mapItemSWU = new Dictionary<int, float>();
        foreach (QMatrixHUSSpan qmatrix in database)
        {
            // for each row (item) we will update the swu of the corresponding item
            foreach (int item in qmatrix.ItemNames)
            {
                // get its swu
                float currentSWU = 0;
                // update its swu
                if (mapItemSWU.ContainsKey(item))
                {
                    currentSWU = mapItemSWU[item];
                    mapItemSWU[item] = currentSWU + qmatrix.Swu;
                }
                else
                {
                    mapItemSWU[item] = qmatrix.Swu;
                }
            }
        }
        foreach (KeyValuePair<int, float> entry in mapItemSWU.SetOfKeyValuePairs())
        {
            if (entry.Value >= minUtility)
            {
                numberOfPatterns++;
                // We get the item
                int item = entry.Key;
                // We initialize two variables for calculating the total utility and remaining utility
                // of that item => PEU(t) = totalUtility + totalRemainingUtility
                float peut = 0;
                float totalUtility = 0;

                // We also initialize a variable to remember the projected qmatrixes of sequences
                // where this item appears. This will be used for call to the recursive
                // "husspan" method later.

                UtilityChainList utilityChains = new UtilityChainList();
                IList<UtilityChain> listofUtility1Chain = new List<UtilityChain>();
                IList<int> seqIdList = new List<int>();
                // For each sequence
                foreach (QMatrixHUSSpan qmatrix in database)
                {
                    //The following variable is used to create the utility chain of sequence t in each q-sequence
                    UtilityList uList = null;
                    UtilityChain utility1Chain = null;
                    // This variable will store the maximum utility and maximum tid
                    //TAI MODIFIES FOR NEGATIVE ITEMS    
                    //float maxUtility = 0;
                    float maxUtility = float.MinValue;
                    // if the item appear in that sequence (in that qmatrix)
                    int row = Array.BinarySearch(qmatrix.ItemNames, item);
                    if (row >= 0)
                    {
                        // for each itemset in that sequence
                        int itemset;
                        float maxpeuts = 0;
                        for (itemset = qmatrix.MatrixItemRemainingUtility[row].Length - 1; itemset >= 0; itemset--)
                        {
                            // get the utility of the item in that itemset
                            float utility = qmatrix.MatrixItemUtility[row][itemset];
                            //TAI MODIFIES FOR NEGATIVE ITEMS
                            // if the utility is higher than 0
                            //if (utility > 0)
                            //{
                            if (utility != 0)
                            {
                                if (utility > maxUtility)
                                {
                                    maxUtility = utility;
                                }
                                float peuts = 0;
                                float remaining = qmatrix.MatrixItemRemainingUtility[row][itemset];
                                if (remaining == 0)
                                    peuts = 0;
                                else
                                    peuts = utility + remaining;
                                if (peuts > maxpeuts)
                                    maxpeuts = peuts;
                                if (uList == null)
                                    uList = new UtilityList(itemset, utility);
                                else
                                {
                                    uList = new UtilityList(itemset, utility, uList);
                                }
                            }
                        }
                        utility1Chain = new UtilityChain(maxpeuts, uList);
                        seqIdList.Add(qmatrix.SeqID);
                        listofUtility1Chain.Add(utility1Chain);
                        // update the peut of 1-sequence t until now by adding the maxpeuts of the current sequence
                        peut += maxpeuts;
                        //Concurently we update the utility of this sequence
                        totalUtility += maxUtility;
                    }
                }
                //numberOfPatterns++;
                utilityChains = new UtilityChainList(seqIdList, listofUtility1Chain);
                // create the pattern consisting of this item
                // by appending the item to the prefix in the buffer, which is empty
                prefix[0] = item;
                // if the pattern is high utility then output it
                if (totalUtility >= minUtility)
                {
                    // writeOut(prefix, 1, totalUtility);
                    patternCount++;
                    int[] items = new int[1];
                    for (int i = 0; i < items.Length; i++)
                    {
                        items[i] = prefix[i];
                    }
                    highUtilitySet.Add(items, totalUtility);
                    //System.Console.WriteLine("********PHUSP: " + arrayToString(items) + ":" + totalUtility.ToString());
                }

                //}
                //Then, we recursively call the procedure husspan for growing this pattern and
                // try to find larger high utility sequential patterns

                // if this item passes the depth pruning (remaining utility + totality >= minutil), i.e PEU(t) >=minutil
                if (peut >= minUtility)
                {
                    if (1 < maxPatternLength)
                    {
                        husspanForPUSOM(prefix, 1, utilityChains, 1);

                    }
                }
            }
        }
    }

    private void husspanForPUSOM(int[] prefix, int prefixLength, UtilityChainList utilityChains, int itemCount)
    {
        // =======================  I-Extension  ===========================/
        // We first try to perform I-Extension to grow the pattern larger.
        // We scan the Utility Chain to find item that could be concatenated to the prefix.
        // For each sequence in the projected database
        IDictionary<int, float> iList = new Dictionary<int, float>();
        IDictionary<int, float> sList = new Dictionary<int, float>();
        int lastItem = prefix[prefixLength - 1];
        //First collecting all the items for i-list
        //And concurrently calculate the RSU(t,s) of all extension sequence
        IList<UtilityChain> listOfUtilityChain = utilityChains.ListOfUtilityChain;
        IList<int> seqIdList = utilityChains.SeqIDList;
        UtilityChain utility1Chain = new UtilityChain();
        for (int x = 0; x < listOfUtilityChain.Count; x++)
        {
            utility1Chain = listOfUtilityChain[x];
            // Get the utility list in the utility chain 
            UtilityList utilityList = utility1Chain.UtilityList;
            QMatrixHUSSpan qmatrix = database[seqIdList[x] - 1];
            //The two temporal list to check whether items are already in ilist and slist
            IList<int> iitems = new List<int>();
            IList<int> sitems = new List<int>();
            do
            {
                int icolumn = utilityList.TID;
                int irow = Array.BinarySearch(qmatrix.getItemNames(), lastItem) + 1;
                for (; irow < qmatrix.getItemNames().Length; irow++)
                {
                    // get the item for this row
                    int item = qmatrix.getItemNames()[irow];
                    float currentRSU;
                    float firstRSU;
                    // if the item appears in that column
                    //TAI MODIFIES FOR NEGATIVE ITEMS    
                    if (qmatrix.getItemUtility(irow, icolumn) != 0)
                    {
                        if (!iitems.Contains(item))
                        {
                            iitems.Add(item);
                            if (iList.ContainsKey(item))
                            {
                                currentRSU = iList[item];
                                currentRSU += utility1Chain.PEUTS;
                                iList[item] = currentRSU;
                            }
                            // if it is the first time that we see this item
                            else
                            {
                                // We use a Pair object to store the SWU of the item and the
                                firstRSU = utility1Chain.PEUTS;
                                iList[item] = firstRSU;
                            }
                        }
                    }
                }

                // For each item
                for (int srow = 0; srow < qmatrix.getItemNames().Length; srow++)
                {
                    // get the item for this row
                    int item = qmatrix.getItemNames()[srow];
                    for (int scolumn = utilityList.TID + 1; scolumn < qmatrix.MatrixItemUtility[srow].Length; scolumn++)
                    {
                        float currentRSU;
                        float firstRSU;
                        // if the item appears in that column
                        //TAI MODIFIES FOR NEGATIVE ITEMS    
                        if (qmatrix.getItemUtility(srow, scolumn) != 0)
                        {
                            if (!sitems.Contains(item))
                            {
                                sitems.Add(item);
                                if (sList.ContainsKey(item))
                                {
                                    currentRSU = sList[item];
                                    currentRSU += utility1Chain.PEUTS;
                                    sList[item] = currentRSU;
                                }
                                // if it is the first time that we see this item
                                else
                                {
                                    // We use a Pair object to store the SWU of the item and the
                                    firstRSU = utility1Chain.PEUTS;
                                    sList[item] = firstRSU;
                                }
                            }
                            break;
                        }
                    }
                }

                utilityList = utilityList.Link;
            } while (utilityList != null);

        }
        //// Now that we have calculated the local RSU of each item,
        ////We perform a loop on each item and for each promising item we will create
        ////the i-concatenation and calculate the utility of the resulting pattern.

        ////For each item
        foreach (KeyValuePair<int, float> entry in iList.SetOfKeyValuePairs())
        {
            // if the item is promising (RSU >= minutil)
            if (entry.Value >= minUtility)
            {
                numberOfPatterns++;
                // get the item
                int item = entry.Key;
                if (prefix[0] == 2 && prefix[1] == 5 && prefix[3] == 1 && prefix[4] == 6 && prefix[6] == 1)
                    Console.Write("Hello");
                // we will traverse the utility chain to calculate the utility 
                // and create the extension utility chain of i-concatenation
                // We initialize two variables for calculating the total utility and PEUT
                // of that item => PEU(t) = totalUtility + totalRemainingUtility
                float peut = 0;
                float totalUtility = 0;
                // Initialize a variable to store the utility chain for the i-concatenation
                // of this item to the prefix
                IList<UtilityChain> listofUtility1ChainIConcatenation = new List<UtilityChain>();
                IList<int> seqIdListIConcatenation = new List<int>();
                UtilityChainList utilityChainsIConcatenation = new UtilityChainList();

                for (int y = 0; y < listOfUtilityChain.Count; y++)
                {
                    utility1Chain = listOfUtilityChain[y];
                    // Get the utility list in the utility chain 
                    UtilityList utilityList = utility1Chain.UtilityList;
                    QMatrixHUSSpan qmatrix = database[seqIdList[y] - 1];
                    // This variable will store the maximum utility and maximum peuts
                    float maxUtility = 0;
                    float maxpeuts = 0;
                    UtilityChain exUtility1Chain = null;
                    //The following variable is used to create the utility list of sequence t in each q-sequence
                    UtilityList finalList = null;
                    IList<UtilityList> tmpList = new List<UtilityList>();
                    int row = Array.BinarySearch(qmatrix.getItemNames(), item);
                    if (row >= 0)
                    {
                        do
                        {
                            int column = utilityList.TID;
                            // get the utility of the item in that itemset
                            float utility = qmatrix.MatrixItemUtility[row][column];
                            // if the utility is higher than 0
                            //TAI MODIFIES FOR NEGATIVE ITEMS    
                            if (utility != 0)
                            {
                                float peuts = 0;
                                float acu = utilityList.ACU + utility;
                                //TAI MODIFIES FOR NEGATIVE ITEMS    
                                if (acu > maxUtility || acu < 0)
                                    maxUtility = acu;
                                float remaining = qmatrix.MatrixItemRemainingUtility[row][column];
                                //if (remaining == 0)
                                //    peuts = 0;
                                //else
                                peuts = acu + remaining;
                                if (peuts > maxpeuts)
                                    maxpeuts = peuts;
                                UtilityList newList = new UtilityList(column, acu);
                                tmpList.Add(newList);
                            }
                            utilityList = utilityList.Link;
                        } while (utilityList != null);
                    }
                    if (tmpList.Count > 0)
                    {
                        for (int i = 0; i < tmpList.Count - 1; i++)
                        {
                            tmpList[i].Link = tmpList[i + 1];
                        }
                        finalList = tmpList[0];
                        exUtility1Chain = new UtilityChain(maxpeuts, finalList);
                        listofUtility1ChainIConcatenation.Add(exUtility1Chain);
                        seqIdListIConcatenation.Add(qmatrix.SeqID);
                    }
                    // update the peut of extension sequence t until now by adding the maxpeuts of the current sequence
                    peut += maxpeuts;
                    //Concurently we update the utility of this sequence
                    totalUtility += maxUtility;
                }
                //numberOfPatterns++;
                utilityChainsIConcatenation = new UtilityChainList(seqIdListIConcatenation, listofUtility1ChainIConcatenation);

                // create the i-concatenation by appending the item to the prefix in the buffer
                prefix[prefixLength] = item;

                // if the i-concatenation is high utility, then output it
                if (totalUtility >= minUtility)
                {

                    //writeOut(prefix, prefixLength + 1, totalUtility);
                    int[] items = new int[prefixLength + 1];
                    for (int i = 0; i < items.Length; i++)
                    {
                        items[i] = prefix[i];
                    }
                    highUtilitySet.Add(items, totalUtility);
                    patternCount++;

                }
                // Finally, we recursively call the procedure husspan for growing this pattern
                // to try to find larger patterns
                //if this i-concatenation passes the depth pruning (remaining utility + totality)
                if (peut >= minUtility)
                {
                    if (itemCount + 1 < maxPatternLength)
                    {
                        husspanForPUSOM(prefix, prefixLength + 1, utilityChainsIConcatenation, itemCount + 1);
                    }
                }
            }
        }

        // =======================  S-Extension  ===========================/
        // We will next look for for S-Extension.
        // Next we will calculate the utility of each s-concatenation for promising 
        // items that can be appended by s-concatenation
        foreach (KeyValuePair<int, float> entry in sList.SetOfKeyValuePairs())
        {
            // if the item is promising (RSU >= minutil)
            if (entry.Value >= minUtility)
            {
                numberOfPatterns++;
                // get the item
                int item = entry.Key;
                if (prefix[0] == 2 && prefix[1] == 5 && prefix[3] == 1 && prefix[4] == 6 && item == 1)
                    Console.Write("Hello");
                // we will traverse the utility chain to calculate the utility 
                // and create the extension utility chain of s-concatenation
                // We initialize two variables for calculating the total utility and PEUT
                // of that item => PEU(t) = totalUtility + totalRemainingUtility
                float peut = 0;
                float totalUtility = 0;
                // Initialize a variable to store the utility chain for the s-concatenation
                // of this item to the prefix
                IList<UtilityChain> listofUtility1ChainSConcatenation = new List<UtilityChain>();
                IList<int> seqIdListSConcatenation = new List<int>();
                UtilityChainList utilityChainsSConcatenation = new UtilityChainList();

                for (int z = 0; z < listOfUtilityChain.Count; z++)
                {
                    utility1Chain = listOfUtilityChain[z];
                    // Get the utility list in the utility chain 
                    UtilityList utilityList = utility1Chain.UtilityList;
                    QMatrixHUSSpan qmatrix = database[seqIdList[z] - 1];
                    // This variable will store the maximum utility and maximum peuts
                    float maxUtility = 0;
                    float maxpeuts = 0;
                    UtilityChain exUtility1Chain = null;
                    //The following variable is used to create the utility list of sequence t in each q-sequence
                    UtilityList finalList = null;
                    IList<UtilityList> tmpList = new List<UtilityList>();
                    int row = Array.BinarySearch(qmatrix.getItemNames(), item);
                    if (row >= 0)
                    {
                        do
                        {
                            for (int column = utilityList.TID + 1; column < qmatrix.MatrixItemUtility[row].Length; column++)
                            {
                                // get the utility of the item in that itemset
                                float utility = qmatrix.MatrixItemUtility[row][column];
                                // if the utility is higher than 0
                                if (utility != 0)
                                {
                                    float peuts = 0;
                                    float acu = utilityList.ACU + utility;
                                    //TAI MODIFIES FOR NEGATIVE ITEMS    
                                    if (acu > maxUtility || acu < 0)
                                        maxUtility = acu;
                                    float remaining = qmatrix.MatrixItemRemainingUtility[row][column];
                                    //if (remaining == 0)
                                    //    peuts = 0;
                                    //else
                                    peuts = acu + remaining;
                                    if (peuts > maxpeuts)
                                        maxpeuts = peuts;
                                    UtilityList newList = new UtilityList(column, acu);
                                    tmpList.Add(newList);
                                }
                            }
                            utilityList = utilityList.Link;
                        } while (utilityList != null);
                    }
                    if (tmpList.Count > 0)
                    {
                        for (int i = 0; i < tmpList.Count - 1; i++)
                        {
                            tmpList[i].Link = tmpList[i + 1];
                        }
                        finalList = tmpList[0];
                        exUtility1Chain = new UtilityChain(maxpeuts, finalList);
                        listofUtility1ChainSConcatenation.Add(exUtility1Chain);
                        seqIdListSConcatenation.Add(qmatrix.SeqID);
                    }
                    // update the peut of extension sequence t until now by adding the maxpeuts of the current sequence
                    peut += maxpeuts;
                    //Concurently we update the utility of this sequence
                    totalUtility += maxUtility;
                }
                //numberOfPatterns++;
                utilityChainsSConcatenation = new UtilityChainList(seqIdListSConcatenation, listofUtility1ChainSConcatenation);
                // create ths s-concatenation by appending an itemset separator to 
                // start a new itemset
                prefix[prefixLength] = -1;
                // then we append the new item
                prefix[prefixLength + 1] = item;


                // if this s-concatenation is high utility, then we output it
                if (totalUtility >= minUtility)
                {

                    //    //writeOut(prefix, prefixLength + 2, totalUtility);
                    int[] items = new int[prefixLength + 2];
                    for (int i = 0; i < items.Length; i++)
                    {
                        items[i] = prefix[i];
                    }
                    highUtilitySet.Add(items, totalUtility);
                    patternCount++;
                    //System.Console.WriteLine("********PHUSP: " + arrayToString(items) + ":" + totalUtility.ToString();
                }
                // Finally, we recursively call the procedure husspan() for growing this pattern
                // to try to find larger high utilit sequential patterns
                //if this s-concatenation passes the depth pruning (remaining utility + totality)
                if (peut >= minUtility)
                {

                    if (itemCount + 1 < maxPatternLength)
                    {
                        husspanForPUSOM(prefix, prefixLength + 2, utilityChainsSConcatenation, itemCount + 1);
                    }
                }
            }
        }
    }
    public void runAlgorithmforPUSOM(string external, string input, float minUntil, string output)
    {
        highUtilitySet = new Dictionary<int[], float>();
        patternCount = 0;
        // record the memory usage of the algorithm
        currentProc = Process.GetCurrentProcess();
        // record the start time of the algorithm
        startTimestamp = DateTimeClass.CurrentUnixTimeMillis();

        // create a writer object to write results to file
        //writer = new System.IO.StreamWriter(output);

        // Mine the database recursively using the husspan procedure
        // This procedure is the husspan procedure optimized for the first recursion
        IList<QMatrixHUSSpan> database = loadDataWithInternalExternal(external, input, minUntil);
        husspanFirstTimeForPUSOM(patternBuffer, 0, database);
        // check the memory usage again and close the file.

        //MemoryLogger.getInstance().checkMemory();
        // close output file

        //writer.Close();
        // record end time
        endTimestamp = DateTimeClass.CurrentUnixTimeMillis();
    }
    #endregion

    //private void writeOut(int[] prefix, int prefixLength, float utility)
    //{
    //    // increase the number of high utility itemsets found
    //    //patternCount++;

    //    StringBuilder buffer = new StringBuilder();

    //    // If the user wants to save in SPMF format
    //    if (SAVE_RESULT_EASIER_TO_READ_FORMAT == false)
    //    {
    //        // append each item of the pattern
    //        for (int i = 0; i < prefixLength; i++)
    //        {
    //            buffer.Append(prefix[i]);
    //            buffer.Append(' ');
    //        }

    //        // append the end of itemset symbol (-1) and end of sequence symbol (-2)
    //        buffer.Append("-1 -2 #UTIL: ");
    //        // append the utility of the pattern
    //        buffer.Append(utility);
    //    }
    //    else
    //    {
    //        // Otherwise, if the user wants to save in a format that is easier to read for debugging.

    //        // Append each item of the pattern
    //        buffer.Append('<');
    //        buffer.Append('(');
    //        for (int i = 0; i < prefixLength; i++)
    //        {
    //            if (prefix[i] == -1)
    //            {
    //                buffer.Append(")(");
    //            }
    //            else
    //            {
    //                buffer.Append(prefix[i]);
    //            }
    //        }
    //        buffer.Append(")>:");
    //        buffer.Append(utility);
    //        Array.Resize(ref prefix, prefixLength);
    //    }

    //    // write the pattern to the output file
    //    writer.WriteLine(buffer.ToString());

    //    // if in debugging mode, then also print the pattern to the console
    //    if (DEBUG)
    //    {
    //        Console.WriteLine(" SAVING : " + buffer.ToString());
    //        Console.WriteLine();
    //    }
    //    // check if the calculated utility is correct by reading the file
    //    // for debugging purpose
    //    //    checkIfUtilityOfPatternIsCorrect(prefix, prefixLength, utility);
    //    //}
    //}
    public void setMaxPatternLength(int maxPatternLength)
    {
        this.maxPatternLength = maxPatternLength;
    }
}