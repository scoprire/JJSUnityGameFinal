// Checking for Possible Matches Efficiently
// The goal is to iterate over the entire board, for each tile, swap it with its adjacent tiles(if any), and check for potential matches. If a match is found, revert the swap and return false immediately. If no matches are found after checking all tiles, return true.

//Step 1: Efficient Swapping and Checking

//To efficiently swap and check, we leverage your FindMatch method, which already does raycasting in a specific direction. However, instead of performing actual swaps, we simulate the swaps by temporarily changing the sprites' references during the match checking process. This minimizes the overhead of physically moving objects in the scene and reverting their positions, which can be expensive.

//Step 2: Implement the Checking Function

//We create a function named CheckForAnyPossibleMatches. This function iterates through the board, for each tile, it checks all adjacent tiles (up, down, left, right) for potential swaps and matches.

public bool CheckForAnyPossibleMatches()
{
    for (int x = 0; x < boardWidth; x++)
    {
        for (int y = 0; y < boardHeight; y++)
        {
            TileTest currentTile = board[x, y].GetComponent<TileTest>();
            foreach (Vector2 direction in adjacentDirections)
            {
                Vector2 adjacentPosition = new Vector2(x, y) + direction;
                if (IsValidPosition(adjacentPosition))
                {
                    // Simulate swap
                    TileTest adjacentTile = board[(int)adjacentPosition.x, (int)adjacentPosition.y].GetComponent<TileTest>();
                    Sprite originalSprite = currentTile.renderer.sprite;
                    currentTile.renderer.sprite = adjacentTile.renderer.sprite;
                    adjacentTile.renderer.sprite = originalSprite;

                    // Check for a match
                    if (currentTile.CheckMatches() || adjacentTile.CheckMatches())
                    {
                        // Revert swap if match found
                        adjacentTile.renderer.sprite = currentTile.renderer.sprite;
                        currentTile.renderer.sprite = originalSprite;
                        return false; // Match found, no need to continue
                    }

                    // Revert swap if no match found
                    adjacentTile.renderer.sprite = currentTile.renderer.sprite;
                    currentTile.renderer.sprite = originalSprite;
                }
            }
        }
    }
    return true; // No possible matches found
}

private bool IsValidPosition(Vector2 position)
{
    return position.x >= 0 && position.x < boardWidth && position.y >= 0 && position.y < boardHeight;
}
