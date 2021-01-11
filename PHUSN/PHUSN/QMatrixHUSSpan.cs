using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class QMatrixHUSSpan
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
    public QMatrixHUSSpan(int nbItem, int nbItemset, int[] itemNames, int itemNamesLength, float swu)
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

    public virtual void registerItem(int itemPos, int itemset, float utility, float remainingUtility)
    {
        // we store the utility in the cell for this item/itemset
        matrixItemUtility[itemPos][itemset] = utility;
        // we store the remaining utility in the cell for this item/itemset
        matrixItemRemainingUtility[itemPos][itemset] = remainingUtility;
    }

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
    public int[] getItemNames()
    {
        return this.ItemNames;
    }
    public float getItemUtility(int row, int column)
    {
        return this.MatrixItemUtility[row][column];
    }
    public float getRemainingUtility(int row, int column)
    {
        return this.MatrixItemRemainingUtility[row][column];
    }
}
