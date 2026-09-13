using UnityEngine;
using System;


/// <summary>
/// Performs the index and world coordinate calculations of a 2D grid on a board.
/// </summary>
/// <remarks>
/// For boards made of squares only.
/// </remarks>
public static class BoardGeometry
{
    // ==================================================
    // Private Fields
    // ==================================================
    private static int numRows;
    private static int numCols;

    private static float cellWidth;
    private static float zeroX;    // adjusted 0 at the left in world coordinates
    private static float zeroY;    // adjusted 0 at the top in world coordinates

    private static bool initialized = false;


    // ==================================================
    // Initialization
    // ==================================================

    /// <summary>
	/// Calculates the cell offset and origin world coordinates.
	/// </summary>
	/// <param name="rows">Number of rows on the board.</param>
	/// <param name="cols">Number of cols on the board.</param>
	/// <param name="cellRadius">Radius of a spot on the board.</param>
	/// <param name="board">Boundaries of box that holds the board.</param>
    public static void Initialize(int rows, int cols, Bounds board)
    {
        if (rows <= 0 || cols <= 0)
            throw new ArgumentOutOfRangeException($"[BoardGeometry] Number of rows ({rows}) and columns ({cols}) must be greater than 0");
        if (board.size == Vector3.zero)
            throw new ArgumentException($"[BoardGeometry] Invalid bounds passed to initializer: {board}");

        numRows = rows;
        numCols = cols;

        float boardLeft = board.min.x;
        float boardTop = board.max.y;
        float boardRight = board.max.x;

        cellWidth = (boardRight - boardLeft) / numCols;
        zeroX = boardLeft + cellWidth / 2;
        zeroY = boardTop - cellWidth / 2;

        initialized = true;
    }


    // ==================================================
    // Public Methods
    // ==================================================

    /// <summary>
	/// Calculates the board index from a position.
	/// Clamps -1 to num rows/num cols for out of bounds values.
	/// </summary>
	/// <param name="position">Transform in world coordinates</param>
	/// <returns>The board index (row, col) corresponding to the board array.</returns>
    public static Vector2Int TransformToBoardIndex(Vector3 position)
    {
        if (!initialized)
            throw new InvalidOperationException("[BoardGeometry] Used before Initialize() succeeded");

        int r = Mathf.Clamp(Mathf.RoundToInt((zeroY - position.y) / cellWidth), -1, numRows);
        int c = Mathf.Clamp(Mathf.RoundToInt((position.x - zeroX) / cellWidth), -1, numCols);

        return new Vector2Int(r, c);
    }

    /// <summary>
	/// Calculates the world position of a cell.
	/// </summary>
	/// <param name="index">Board index of the cell, (row, col).</param>
	/// <returns>The world positon of the cell.</returns>
    public static Vector3 BoardIndexToTransform(Vector2Int index)
    {
        if (!initialized)
            throw new InvalidOperationException("[BoardGeometry] Used before Initialize() succeeded");

        Vector3 newTransform = Vector3.zero;

        newTransform.x = zeroX + index.y * cellWidth;
        newTransform.y = zeroY - index.x * cellWidth;

        return newTransform;
    }
}
