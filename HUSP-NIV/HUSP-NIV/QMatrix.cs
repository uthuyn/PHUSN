using System;
using System.Text;

public class QMatrix
{

    int seqID;

    public int SeqID
    {
        get { return seqID; }
        set { seqID = value; }
    }
    /// <summary>
    /// the qmatrix for items  [item][itemset] -> utility </summary>
    float[][] matrixItemUtility;

    public float[][] MatrixItemUtility
    {
        get { return matrixItemUtility; }
        set { matrixItemUtility = value; }
    }
    /// <summary>
    /// the qmatrix for remaining utility [item][itemset] -> remaining utility </summary>
    float[][] matrixItemRemainingUtility;

    public float[][] MatrixItemRemainingUtility
    {
        get { return matrixItemRemainingUtility; }
        set { matrixItemRemainingUtility = value; }
    }
    /// <summary>
    /// the item names </summary>
    int[] itemNames;

    public int[] ItemNames
    {
        get { return itemNames; }
        set { itemNames = value; }
    }
    //number of Itemset
    int nbItemsets;

    public int NbItemsets
    {
        get { return nbItemsets; }
        set { nbItemsets = value; }
    }
    /// <summary>
    /// the swu of this sequence * </summary>
    float swu;

    public float Swu
    {
        get { return swu; }
        set { swu = value; }
    }

    /// <summary>
    /// Constructor </summary>
    /// <param name="nbItem"> the number of item in the sequence </param>
    /// <param name="nbItemset"> the number of itemsets in that sequence </param>
    public QMatrix(int nbItem, int nbItemset, int[] itemNames, int itemNamesLength,float swu)
    {
        matrixItemUtility = RectangularArrays.ReturnRectangularIntArray(nbItem, nbItemset);
        matrixItemRemainingUtility = RectangularArrays.ReturnRectangularIntArray(nbItem, nbItemset);
        this.swu = swu;

        this.itemNames = new int[itemNamesLength];
        Array.Copy(itemNames, 0, this.itemNames, 0, itemNamesLength);
        //for (int i = 0; i < itemNamesLength;i++)
        //{
        //    this.itemNames[i] = itemNames[i];
        //}
    }

    /// <summary>
    /// Register item in the matrix </summary>
    /// <param name="itemPos"> an item position in "itemNames" </param>
    /// <param name="itemset"> the itemset number </param>
    /// <param name="utility"> the utility of the item in that itemset </param>
    /// <param name="remainingUtility"> the reamining utility of that item at that itemset </param>
    public virtual void registerItem(int itemPos, int itemset, float utility, float remainingUtility)
    {
        // we store the utility in the cell for this item/itemset
        matrixItemUtility[itemPos][itemset] = utility;
        // we store the remaining utility in the cell for this item/itemset
        matrixItemRemainingUtility[itemPos][itemset] = remainingUtility;
    }


    /// <summary>
    /// Get a string representation of this matrix (for debugging purposes) </summary>
    /// <returns> the string representation </returns>
    public override string ToString()
    {
        StringBuilder buffer = new StringBuilder();
        buffer.Append(" MATRIX \n");
        for (int i = 0; i < itemNames.Length; i++)
        {
            buffer.Append("\n  item: " + itemNames[i] + "  ");
            for (int j = 0; j < matrixItemUtility[i].Length; j++)
            {
                buffer.Append("  " + matrixItemUtility[i][j] + "[" + +matrixItemRemainingUtility[i][j] + "]");
            }
        }
        buffer.Append("   swu: " + swu);
        buffer.Append("\n");
        return buffer.ToString();
    }
}
