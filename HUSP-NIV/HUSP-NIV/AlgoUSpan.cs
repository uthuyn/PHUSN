                                                                                                                                                                                                                                                                     using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

public class AlgoUSpan
{
    /// <summary>
    /// if true, debugging information will be shown in the console </summary>
    readonly bool DEBUG = true;

    #region "Declaration Variable"
    /// <summary>
    /// the time the algorithm started </summary>
    public long startTimestamp = 0;
    public long startTimestampMSPCF = 0;
    /// <summary>
    /// the time the algorithm terminated </summary>
    public long endTimestamp = 0;
    public long endTimestampMSICF = 0;
    /// <summary>
    /// record memory usage the algorithm terminated </summary>
    public Process currentProc;
    /// <summary>
    /// the number of HUI generated </summary>
    public int patternCount = 0;

    /// <summary>
    /// writer to write the output file * </summary>
    StreamWriter writer = null;

    /// <summary>
    /// buffer for storing the current pattern that is mined when performing mining
    /// the idea is to always reuse the same buffer to reduce memory usage. *
    /// </summary>
    readonly int BUFFERS_SIZE = 2000;
    private int[] patternBuffer = null;

    /// <summary>
    /// if true, save result to file in a format that is easier to read by humans * </summary>
    readonly bool SAVE_RESULT_EASIER_TO_READ_FORMAT = true;

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
    IList<QMatrix> database;
    /// <summary>
    /// Default constructor
    /// </summary>
    public AlgoUSpan()
    {

    }
    //List to save High utility pattern 
    public Dictionary<Dictionary<int[], IList<QMatrixProjection>>, float> highUtilitySet = new Dictionary<Dictionary<int[], IList<QMatrixProjection>>, float>();
    /// <summary>
    /// Run the USPAN algorithm </summary>
    /// <param name="input"> the input file path </param>
    /// <param name="output"> the output file path </param>
    /// <param name="minUtility"> the minimum utility threshold </param>
    /// <exception cref="IOException"> exception if error while writing the file </exception>
    #endregion

    #region"Mining Phase"
    public void setMaxPatternLength(int maxPatternLength)
    {
        this.maxPatternLength = maxPatternLength;
    }
    public void runAlgorithm(string external, string input, float minUntil, string output)
    {
        highUtilitySet = new Dictionary<Dictionary<int[], IList<QMatrixProjection>>, float>();
        patternCount = 0;
        // record the memory usage of the algorithm
        currentProc = Process.GetCurrentProcess();
        // record the start time of the algorithm
        startTimestamp = DateTimeClass.CurrentUnixTimeMillis();

        // create a writer object to write results to file
        writer = new System.IO.StreamWriter(output);
        // Mine the database recursively using the USpan procedure
        // This procedure is the USPan procedure optimized for the first recursion
        IList<QMatrix> database = loadDataWithInternalExternal(external, input, minUntil);
        uspanFirstTime(patternBuffer, 0, database);

        // check the memory usage again and close the file.

        //MemoryLogger.getInstance().checkMemory();
        // close output file
        writer.Close();
        // record end time
        endTimestamp = DateTimeClass.CurrentUnixTimeMillis();
    }
    #endregion

    #region "USpan First Time"
    /// <summary>
    /// This is the initial call to the USpan procedure to find all High utility sequential patterns
    /// of length 1. It is optimized for finding patterns of length 1. 
    /// To find larger patterns the "uspan" method is then used recursively. </summary>
    /// <param name="prefix">  This is the buffer for storing the current prefix. Initially, it is empty. </param>
    /// <param name="prefixLength"> The current prefix length. Initially, it is zero. </param>
    /// <param name="database"> This is the original sequence database (as a set of QMatrix) </param>
    /// <exception cref="IOException"> If an error occurs while reading/writting to file. </exception>
    private void uspanFirstTime(int[] prefix, int prefixLength, IList<QMatrix> database)
    {

        // For the first call to USpan, we only need to check I-CONCATENATIONS
        // =======================  I-CONCATENATIONS  ===========================/
        // scan the projected database to
        // calculate the SWU of each item
        //startTimestamp = DateTimeHelperClass.CurrentUnixTimeMillis();
        IDictionary<int, float> mapItemSWU = new Dictionary<int, float>();
        foreach (QMatrix qmatrix in database)
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

        // For each item 
        foreach (KeyValuePair<int, float> entry in mapItemSWU.SetOfKeyValuePairs())
        {
            float itemSWU = entry.Value;
            // if the item is promising
            if (itemSWU >= minUtility)
            {
                // We get the item
                int item = entry.Key;
                // We initialize two variables for calculating the total utility and remaining utility
                // of that item
                float totalUtility = 0;
                float totalRemainingUtility = 0;

                // We also initialize a variable to remember the projected qmatrixes of sequences
                // where this item appears. This will be used for call to the recursive
                // "uspan" method later.
                IList<QMatrixProjection> matrixProjections = new List<QMatrixProjection>();
                // For each sequence
                foreach (QMatrix qmatrix in database)
                {

                    // if the item appear in that sequence (in that qmatrix)
                    int row = Array.BinarySearch(qmatrix.ItemNames, item);
                    if (row >= 0)
                    {

                        // create a list to store the positions (itemsets) where this item 
                        // appear in that sequence
                        IList<MatrixPosition> positions = new List<MatrixPosition>();

                        // find the max utility of this item in that sequence
                        // and the max remaining utility
                        //TAI MODIFIES FOR NEGATIVE ITEMS    
                        //float maxUtility = 0;
                        float maxUtility = float.MinValue;
                        float maxRemainingUtility = 0;

                        // for each itemset in that sequence
                        for (int itemset = 0; itemset < qmatrix.MatrixItemRemainingUtility[row].Length; itemset++)
                        {
                            // get the utility of the item in that itemset
                            float utility = qmatrix.MatrixItemUtility[row][itemset];
                            // if the utility is higher than 0
                            //TAI MODIFIES FOR NEGATIVE ITEMS         
                            //if (utility > 0)
                            //{
                            // record this position
                            if (utility !=0)
                            {
                                positions.Add(new MatrixPosition(row, itemset, utility));
                            }
                            // if it is the maximum utility until now
                            if (utility != 0 && utility > maxUtility)
                            {
                                // record it as the maximum utility until now
                                maxUtility = utility;

                                // get the remaining utility at that position
                                float remaining = qmatrix.MatrixItemRemainingUtility[row][itemset];
                                // If it is the first occurrence of this item
                                // we remember the remaining utility as the max remaining utility
                                if (remaining > 0 && maxRemainingUtility == 0)
                                {
                                    maxRemainingUtility = remaining;
                                }
                            }
                            //}
                        }

                        // update the total utility and total remaining utility for all sequences 
                        // until now by adding the utility and remaining utility of the current
                        // sequence
                        totalUtility += maxUtility;
                        totalRemainingUtility += maxRemainingUtility;

                        // create the projected matrix for the current sequence
                        QMatrixProjection projection = new QMatrixProjection(qmatrix, positions);
                        // add the projected matrix to the current database projection with this item
                        matrixProjections.Add(projection);
                    }
                }


                // create the pattern consisting of this item
                // by appending the item to the prefix in the buffer, which is empty
                prefix[0] = item;
                // if the pattern is high utility, then output it
                if (totalUtility >= minUtility)
                {
                    //writeOut(prefix, 1, totalUtility);
					patternCount++;
                    Dictionary<int[], IList<QMatrixProjection>> key = new Dictionary<int[], IList<QMatrixProjection>>();
                    int[] items = new int[1];
                    for (int i = 0; i < items.Length; i++)
                    {
                        items[i] = prefix[i];
                    }
                    key.Add(items, matrixProjections);
                    highUtilitySet.Add(key, totalUtility);
                }

                //Then, we recursively call the procedure uspan for growing this pattern and
                // try to find larger high utility sequential patterns
                // if this item passes the depth pruning (remaining utility + totality >= minutil)
                if (totalUtility + totalRemainingUtility >= minUtility)
                {
                    if (1 < maxPatternLength)
                    {
                        uspan(prefix, 1, matrixProjections, 1);
                    }
                }
            }
        }
        // we check the memory usage.
        //endTimestamp = DateTimeHelperClass.CurrentUnixTimeMillis();
        //MemoryLogger.getInstance().checkMemory();
    }


    /// <summary>
    /// This inner class is used to store the SWU of an item and the last sequence where it was seen.
    /// It is used in the uspan() method.
    /// </summary>
    private class Pair
    {
        private readonly AlgoUSpan outerInstance;

        public Pair(AlgoUSpan outerInstance)
        {
            this.outerInstance = outerInstance;
        }

        /// <summary>
        /// the total SWU of an item </summary>
        internal float swu;
        /// <summary>
        /// the last sequence where the item was seen  (represented by a projected QMatrix) </summary>
        internal QMatrixProjection lastSID;
    }
    #endregion

    #region "Uspan Recursive"
    /// <summary>
    /// This is the general USpan procedure to find all High utility sequential patterns of length
    /// greater than 1. </summary>
    /// <param name="prefix">  This is the buffer for storing the current prefix. </param>
    /// <param name="prefixLength"> The current prefix length. </param>
    /// <param name="database"> This is a projected sequence database (a set of projected QMatrixes) </param>
    /// <param name="itemCount"> the number of items in the prefix </param>
    /// <exception cref="IOException"> If an error occurs while reading/writting to file. </exception>
    private void uspan(int[] prefix, int prefixLength, IList<QMatrixProjection> projectedDatabase, int itemCount)
    {
        // =======================  I-CONCATENATIONS  ===========================/
        // We first try to perform I-Concatenations to grow the pattern larger.
        // We scan the projected database to calculated the SWU of each item that could
        // be concatenated to the prefix.
        // The following map will store for each item, their SWU (key: item  value: swu)
        IDictionary<int, float> mapItemSWU = new Dictionary<int, float>();
        // For each sequence in the projected database
        foreach (QMatrixProjection qmatrix in projectedDatabase)
        {
            IList<int> iitems = new List<int>();
            // For each position in that Q-matrix
            foreach (MatrixPosition position in qmatrix.Positions)
            {
                // Because we are looking for i-concatenation, we will search for items
                // occurring in the same column (itemset) as the current position 
                // but from the next row
                int row = position.row + 1;
                int column = position.column;

                // The sequence utility for updating the SWU
                // will be the remaining utility at the current position
                float localSequenceUtility = qmatrix.getLocalSequenceUtility(position);

                // for each row we will update the local SWU of the corresponding item
                for (; row < qmatrix.getItemNames().Length; row++)
                {
                    // get the item for this row
                    int item = qmatrix.getItemNames()[row];

                    // if the item appears in that column
                    //TAI MODIFIES FOR NEGATIVE ITEMS
                    if (qmatrix.getItemUtility(row, column) != 0)
                    {
                        if (!iitems.Contains(item))
                        {
                            iitems.Add(item);
                            // get its swu until now
                            float currentSWU;
                            if (mapItemSWU.ContainsKey(item))
                            {
                                currentSWU = mapItemSWU[item];
                                mapItemSWU[item] += qmatrix.OriginalMatrix.Swu;
                            }
                            // if it is the first time that we see this item
                            else
                            {
                                mapItemSWU[item] = qmatrix.OriginalMatrix.Swu;
                            }
                        }
                    }
                }
            }
        }
        //  Now that we have calculated the local SWU of each item,
        // We perform a loop on each item and for each promising item we will create
        // the i-concatenation and calculate the utility of the resulting pattern.

        // For each item
        foreach (KeyValuePair<int, float> entry in mapItemSWU.SetOfKeyValuePairs())
        {
            // Get the Pair object that store the calculated SWU for that item
            float itemSWU = entry.Value;
            // if the item is promising (SWU >= minutil)
            if (itemSWU >= minUtility)
            {
                // get the item
                int item = entry.Key;

                // This variable will be used to calculate this item's utility for the whole database
                float totalUtility = 0;
                // This variable will be used to calculate this item's remaining utility for the whole database
                float totalRemainingUtility = 0;

                // Initialize a variable to store the projected QMatrixes for the i-concatenation
                // of this item to the prefix
                IList<QMatrixProjection> matrixProjections = new List<QMatrixProjection>();

                // for each sequence in the projected database
                foreach (QMatrixProjection qmatrix in projectedDatabase)
                {

                    // if the item appear in that sequence
                    int rowItem = Array.BinarySearch(qmatrix.getItemNames(), item);
                    if (rowItem >= 0)
                    {

                        // We initialize two variables that will be used to calculate the maximum
                        // utility and remaining utility for the i-concatenation with this item 
                        // in that sequence
                        float maxUtility = 0;
                        float maxRemainingUtility = 0;

                        // create a list to store the matrix positions of i-concatenations with
                        // this item in that sequence
                        IList<MatrixPosition> positions = new List<MatrixPosition>();

                        // for each position of the prefix
                        foreach (MatrixPosition position in qmatrix.Positions)
                        {
                            // We will look for this item in the same column (in the same itemset)
                            // because we look for a i-concatenation
                            int column = position.column;

                            // we will check if the new item appears in the same itemset
                            float newItemUtility = qmatrix.getItemUtility(rowItem, column);
                            // if the item appears in that itemset
                            //TAI MODIFIES FOR NEGATIVE ITEMS
                            if (newItemUtility != 0)
                            {
                                // calculate the utility of the i-concatenation at this position
                                // in that sequence
                                float newPrefixUtility = position.utility + newItemUtility;
                                // Add this new position and its utility in the list of position
                                // for this pattern
                                positions.Add(new MatrixPosition(rowItem, column, newPrefixUtility));

                                // If the utility of this new i-concatenation is higher than
                                // previous occurrences of that same pattern
                                //TAI MODIFIES FOR NEGATIVE ITEMS
                                if (newPrefixUtility > maxUtility || newPrefixUtility < 0)
                                {
                                    // record this utility as the maximum utility until now for that pattern
                                    maxUtility = newPrefixUtility;

                                    // Get the remaining utility at that position
                                    float remaining = qmatrix.getRemainingUtility(rowItem, column);

                                    // If it is the first position where this i-concatenation occurs
                                    // we record its remaining utility as the largest remaining utility
                                    // for this i-concatenation
                                    if (remaining > 0 && maxRemainingUtility == 0)
                                    {
                                        maxRemainingUtility = remaining;
                                    }
                                }
                            }
                        }

                        // update the total utility and total remaining utility for that i-concatenation
                        // for all sequences by adding the utility and remaining utility for the
                        // current sequence
                        totalUtility += maxUtility;
                        totalRemainingUtility += maxRemainingUtility;

                        // create the projected matrix for the current sequence
                        QMatrixProjection projection = new QMatrixProjection(qmatrix, positions);
                        // Add it to the projected database for that i-concatenation.
                        matrixProjections.Add(projection);
                    }
                }


                // create the i-concatenation by appending the item to the prefix in the buffer
                prefix[prefixLength] = item;
                // if the i-concatenation is high utility, then output it
                if (totalUtility >= minUtility)
                {
                    //writeOut(prefix, prefixLength + 1, totalUtility);
					patternCount++;
                    Dictionary<int[], IList<QMatrixProjection>> key = new Dictionary<int[], IList<QMatrixProjection>>();
                    int[] items = new int[prefixLength + 1];
                    for (int i = 0; i < items.Length; i++)
                    {
                        items[i] = prefix[i];
                    }
                    key.Add(items, matrixProjections);
                    highUtilitySet.Add(key, totalUtility);

                }

                // Finally, we recursively call the procedure uspan for growing this pattern
                // to try to find larger patterns
                // if his i-concatenation passes the depth pruning (remaining utility + totality)
                if (totalUtility + totalRemainingUtility >= minUtility)
                {
                    if (itemCount + 1 < maxPatternLength)
                    {
                        uspan(prefix, prefixLength + 1, matrixProjections, itemCount + 1);
                    }

                }
            }
        }

        // =======================  S-CONCATENATIONS  ===========================/
        // We will next look for for S-CONCATENATIONS.
        // We first clear the map for calculating the SWU of items to reuse it instead
        // of creating a new one
        mapItemSWU.Clear();
        // Now, we will loop over sequences of the projected database to calculate the local SWU
        // of each item.
        //For each sequence in the projected database
        foreach (QMatrixProjection qmatrix in projectedDatabase)
        {
            IList<int> sitems = new List<int>();
            // For each position where the current prefix appear in the current sequence
            foreach (MatrixPosition position in qmatrix.Positions)
            {

                // The local sequence utility is the remaining utility at the current position
                float localSequenceUtility = qmatrix.getLocalSequenceUtility(position);

                // For each item
                for (int row = 0; row < qmatrix.getItemNames().Length; row++)
                {
                    // get the item
                    int item = qmatrix.getItemNames()[row];
                    int column = position.column;
                    //if (prefix[0] == 1 && prefixLength == 1 && qmatrix.OriginalMatrix.SeqID == 0 && item == 1)
                    //    Console.Write("Hello");
                    // We will look for s-concatenations starting from the next itemset (column)
                    // For each column
                    for (column = column + 1; column < qmatrix.OriginalMatrix.MatrixItemUtility[row].Length; column++)
                    {
                        // if the item appears in that column
                        if (qmatrix.getItemUtility(row, column) != 0)
                        {
                            if (!sitems.Contains(item))
                            {
                                sitems.Add(item);
                                // get its swu until now
                                float currentSWU;
                                if (mapItemSWU.ContainsKey(item))
                                {
                                    currentSWU = mapItemSWU[item];
                                    mapItemSWU[item] = currentSWU + qmatrix.OriginalMatrix.Swu;
                                }
                                else
                                {
                                    mapItemSWU[item] = qmatrix.OriginalMatrix.Swu;
                                }
                            }
                            // we don't need to check the other column if we found one column where this item
                            // appears after the previous item.
                            break;
                        }
                    }
                }
            }
        }
        // Next we will calculate the utility of each s-concatenation for promising 
        // items that can be appended by s-concatenation
        foreach (KeyValuePair<int, float> entry in mapItemSWU.SetOfKeyValuePairs())
        {
            // System.out.println(entry.getKey() + "  swu: " + entry.getValue().swu);
            // Get the item and its SWU
            float itemSWU = entry.Value;
            // if the item is promising (SWU >= minutil
            if (itemSWU >= minUtility)
            {

                // get the item
                int item = entry.Key;

                if (prefix[0] == 2 && prefixLength == 1 && item == 3)
                    Console.Write("Hello");

                // This variable is used to store the utility of this s-concatenation in the whole database
                float totalUtility = 0;

                // This variable is used to store the remaining utility of this 
                // s-concatenation in the whole database
                float totalRemainingUtility = 0;

                // Initialize a variable to store position of the last item of this s-concatenation
                IList<QMatrixProjection> matrixProjections = new List<QMatrixProjection>();

                // For each sequence of the projected database,
                // we check if the item appears and calculate the maximum utility
                foreach (QMatrixProjection qmatrix in projectedDatabase)
                {

                    // if the item appear in that qMatrix
                    int rowItem = Array.BinarySearch(qmatrix.getItemNames(), item);
                    if (rowItem >= 0)
                    {

                        // We will calculate the max utility and maximum remaining utility of that
                        // s-concatenation with the item
                        // This variable will store the maximum utility
                        float maxUtility = 0;
                        // This variable will store the maximum remaining utility
                        float maxRemainingUtility = 0;

                        // create a list to store the matrix positions of this item in that sequence
                        IList<MatrixPosition> positions = new List<MatrixPosition>();

                        // for each position of the last item of the prefix
                        foreach (MatrixPosition position in qmatrix.Positions)
                        {

                            // We will start searching for the item starting at the next itemset (column)
                            // because we are looking for s-concatenations					
                            // For each column (itemset) after the last item of the prefix
                            for (int column = position.column + 1; column < qmatrix.OriginalMatrix.MatrixItemUtility[rowItem].Length; column++)
                            {

                                // we will check if the new item appears in the same itemset
                                float newItemUtility = qmatrix.getItemUtility(rowItem, column);
                                // if the item appears
                                if (newItemUtility != 0)
                                {
                                    // We calculate the utility of the s-concatenation with this item
                                    float newPrefixUtility = position.utility + newItemUtility;
                                    // We record the current position as the end of this
                                    // s-concatenation
                                    positions.Add(new MatrixPosition(rowItem, column, newPrefixUtility));

                                    // if this s-concatenation utility is higher than 
                                    // what we seen until now for that sequence
                                    //TAI MODIFIES FOR NEGATIVE ITEMS
                                    if (newPrefixUtility > maxUtility || newPrefixUtility <0)
                                    {
                                        // We record the utiltiy of this s concatenation
                                        maxUtility = newPrefixUtility;

                                        // we get the remaining utility of this s-concatenation
                                        float remaining = qmatrix.getRemainingUtility(rowItem, column);
                                        // if it is the first s-concatenation that we have seen
                                        // we remember the remaining utility
                                        if (remaining > 0 && maxRemainingUtility == 0)
                                        {
                                            maxRemainingUtility = remaining;
                                        }
                                    }
                                }
                            }
                        }

                        // Add the  utility and  remaining utility of this s-concatenation in 
                        // that sequence to the utility and remaining utility of this s-concatenation
                        // for the whole database.
                        totalUtility += maxUtility;
                        totalRemainingUtility += maxRemainingUtility;

                        // create the projected matrix for this s-concatenation
                        QMatrixProjection projection = new QMatrixProjection(qmatrix, positions);
                        // add it to the projected database
                        matrixProjections.Add(projection);
                    }
                }


                // create ths s-concatenation by appending an itemset separator to 
                // start a new itemset
                prefix[prefixLength] = -1;
                // then we append the new item
                prefix[prefixLength + 1] = item;
                // if this s-concatenation is high utility, then we output it
                if (totalUtility >= minUtility)
                {
                    //writeOut(prefix, prefixLength + 2, totalUtility);
					patternCount++;
                    Dictionary<int[], IList<QMatrixProjection>> key = new Dictionary<int[], IList<QMatrixProjection>>();
                    int[] items = new int[prefixLength + 2];
                    for (int i = 0; i < items.Length; i++)
                    {
                        items[i] = prefix[i];
                    }
                    key.Add(items, matrixProjections);
                    highUtilitySet.Add(key, totalUtility);
                }

                // Finally, we recursively call the procedure uspan() for growing this pattern
                // to try to find larger high utilit sequential patterns
                // if this s-concatenation passes the depth pruning 
                // (remaining utility + totality >= minutil)
                if (totalUtility + totalRemainingUtility >= minUtility)
                {
                    if (itemCount + 1 < maxPatternLength)
                    {
                        uspan(prefix, prefixLength + 2, matrixProjections, itemCount + 1);
                    }
                }
            }
        }
        // We check the memory usage

        //MemoryLogger.getInstance().checkMemory();
    }

    /// <summary>
    /// Set the maximum pattern length </summary>
    /// <param name="maxPatternLength"> the maximum pattern length </param>
    public virtual int MaxPatternLength
    {
        set
        {
            this.maxPatternLength = value;
        }
    }

    /// <summary>
    /// Method to write a high utility itemset to the output file. </summary>
    /// <param name="the"> prefix to be written o the output file </param>
    /// <param name="utility"> the utility of the prefix concatenated with the item </param>
    /// <param name="prefixLength"> the prefix length </param>
    private void writeOut(int[] prefix, int prefixLength, float utility)
    {
        // increase the number of high utility itemsets found
        patternCount++;
        if (patternCount == 63)
        {
            Console.WriteLine("find");
        }

        StringBuilder buffer = new StringBuilder();

        // If the user wants to save in SPMF format
        if (SAVE_RESULT_EASIER_TO_READ_FORMAT == false)
        {
            // append each item of the pattern
            for (int i = 0; i < prefixLength; i++)
            {
                buffer.Append(prefix[i]);
                buffer.Append(' ');
            }

            // append the end of itemset symbol (-1) and end of sequence symbol (-2)
            buffer.Append("-1 -2 #UTIL: ");
            // append the utility of the pattern
            buffer.Append(utility);
        }
        else
        {
            // Otherwise, if the user wants to save in a format that is easier to read for debugging.

            // Append each item of the pattern
            buffer.Append('<');
            buffer.Append('(');
            for (int i = 0; i < prefixLength; i++)
            {
                if (prefix[i] == -1)
                {
                    buffer.Append(")(");
                }
                else
                {
                    buffer.Append(prefix[i]);
                }
            }
            buffer.Append(")>:");
            buffer.Append(utility);
            Array.Resize(ref prefix, prefixLength);
        }

        // write the pattern to the output file
        writer.WriteLine(buffer.ToString());

        // if in debugging mode, then also print the pattern to the console
        if (DEBUG)
        {
            Console.WriteLine(" SAVING : " + buffer.ToString());
            Console.WriteLine();
        }
        // check if the calculated utility is correct by reading the file
        // for debugging purpose
        //    checkIfUtilityOfPatternIsCorrect(prefix, prefixLength, utility);
        //}
    }
    #endregion

    public virtual void miningPrintStats()
    {
        Console.WriteLine("=============  USPAN ALGORITHM ==========");
        Console.WriteLine(" Total time ~ " + (endTimestamp - startTimestamp) + " ms");
        Console.WriteLine(" High-utility sequential pattern count : " + patternCount);
        Console.WriteLine(" Memory Usage:" + (currentProc.PrivateMemorySize64 / 1024) / 1024 + "Mb");
        Console.WriteLine("=========================================");
    }

    #region "Load data with internal and external"
    Dictionary<int, float> externalData;
    public IList<QMatrix> loadDataWithInternalExternal(string external, string input, float minUtility)
    {
        //Read External value
        externalData = new Dictionary<int, float>();
        System.IO.StreamReader externalInput = null;
        string line;
        // prepare the object for reading the file
        try
        {
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
        }
        catch (Exception e)
        {
            // catches exception if error while reading the input file
            Console.WriteLine("No data path found");
        }
        finally
        {
            if (externalInput != null)
            {
                externalInput.Close();
            }
        }
        // input path
        this.input = input;

        // initialize the buffer for storing the current itemset
        patternBuffer = new int[BUFFERS_SIZE];

        // save the minimum utility threshold
        this.minUtility = minUtility;

        // create a map to store the SWU of each item
        // key: item  value: the swu of the item
        Dictionary<int, float> mapItemToSWU = new Dictionary<int, float>();


        // ==========  FIRST DATABASE SCAN TO IDENTIFY PROMISING ITEMS =========
        // We scan the database a first time to calculate the SWU of each item.
        int sequenceCount = 0;
        System.IO.StreamReader myInput = null;
        string thisLine;
        float dataSum = 0;
        try
        {
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
                dataSum += sequenceUtility;
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
        }
        catch (Exception e)
        {
            // catches exception if error while reading the input file
            Console.WriteLine("No data path found");
            //Console.Write(e.StackTrace);
        }
        finally
        {
            if (myInput != null)
            {
                myInput.Close();
            }
        }
        //MessageBox.Show(dataSum.ToString());
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

        //================  SECOND DATABASE SCAN ===================
        // Read the database again to create the QMatrix for each sequence
        database = new List<QMatrix>(sequenceCount);

        try
        {
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
            int seqID = -1;
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
                        float itemUtility = float.Parse(internalString) * externalData[item];

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
                    ++seqID;
                    continue;
                }

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

                // New we count the number of items in that sequence
                int nbItems = newItemsPos;

                // And we will create the Qmatrix for that sequence
                QMatrix matrix = new QMatrix(nbItems, --nbItemsets, itemsSequenceBuffer, newItemsPos, sequenceUtility);
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
        }
        catch (Exception e)
        {
            // catches exception if error while reading the input file
            Console.WriteLine("No data path found");
            //Console.Write(e.StackTrace);
        }
        finally
        {
            if (myInput != null)
            {
                myInput.Close();
            }
        }
        return database;
    }
    #endregion

}