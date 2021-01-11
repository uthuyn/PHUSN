using System.Collections.Generic;

public class QMatrixProjection
{

	/// <summary>
	/// the qmatrix for items  [item, utility] </summary>
    QMatrix originalMatrix;

    public QMatrix OriginalMatrix
    {
        get { return originalMatrix; }
        set { originalMatrix = value; }
    }

	/// <summary>
	/// the row of the last added item </summary>
    IList<MatrixPosition> positions;

    public IList<MatrixPosition> Positions
    {
        get { return positions; }
        set { positions = value; }
    }
	/// <summary>
	/// Constructor </summary>
	/// <param name="nbItem"> the number of item in the sequence </param>
	/// <param name="nbItemset"> the number of itemsets in that sequence </param>
	public QMatrixProjection(QMatrix matrix, IList<MatrixPosition> positions)
	{
		this.originalMatrix = matrix;
		this.positions = positions;
	}
	/// <summary>
	/// Constructor </summary>
	/// <param name="nbItem"> the number of item in the sequence </param>
	/// <param name="nbItemset"> the number of itemsets in that sequence </param>
	public QMatrixProjection(QMatrixProjection projection, IList<MatrixPosition> positions)
	{
		this.originalMatrix = projection.originalMatrix;
		this.positions = positions;
	}
   
	/// <summary>
	/// Get the array of items stored in the original Q-matrix </summary>
	/// <returns> the array of items </returns>
    public int[] getItemNames()
    {
        return originalMatrix.ItemNames;
    }

	/// <summary>
	/// Get the local sequence utility for a given cell in the projected q-matrix </summary>
	/// <param name="position"> the cell position (row,column) </param>
	/// <returns> the local sequence utility </returns>
	public float getLocalSequenceUtility(MatrixPosition position)
	{
		return originalMatrix.MatrixItemRemainingUtility[position.row][position.column];
	}

	/// <summary>
	/// Get the utility of a cell in the projected q-matrix at a given cell position (row,column) </summary>
	/// <param name="position"> the position (row, column) </param>
	/// <returns> the utility </returns>
	public float getItemUtility(MatrixPosition position)
	{
		return originalMatrix.MatrixItemUtility[position.row][position.column];
	}

	/// <summary>
	/// Get the utility of a cell in the projected q-matrix at a given cell position (row,column) </summary>
	/// <param name="row"> the row </param>
	/// <param name="column"> the column </param>
	/// <returns> the utility </returns>
	public float getItemUtility(int row, int column)
	{
		return originalMatrix.MatrixItemUtility[row][column];
	}

	/// <summary>
	/// Get the remaining utility of a cell in the projected q-matrix at a given 
	/// cell position (row,column). </summary>
	/// <param name="row"> the row </param>
	/// <param name="column"> the column </param>
	/// <returns> the remaining utility </returns>
	public float getRemainingUtility(int row, int column)
	{
		return originalMatrix.MatrixItemRemainingUtility[row][column];
	}

}
