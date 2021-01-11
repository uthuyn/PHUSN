public class MatrixPosition
{

	/// <summary>
	/// a row </summary>
	public int row;

	/// <summary>
	/// a column </summary>
	public int column;

	/// <summary>
	/// the utility of the prefix ending at this cell (row/column) * </summary>
	public float utility;

	/// <summary>
	/// Constructor </summary>
	/// <param name="row"> the row </param>
	/// <param name="column"> the column </param>
	/// <param name="utility"> the utility </param>
	public MatrixPosition(int row, int column, float utility)
	{
		this.row = row;
		this.column = column;
		this.utility = utility;
	}

}
