using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;
using Unity.VisualScripting;

public class PerlinNoise2D
{
    private System.Random r;
    private Vector2[] gradients;
    private int[] perm;
    public PerlinNoise2D(int seed, int permutationLength)
    {
        r = new System.Random(seed);
        gradients = new Vector2[permutationLength];
        perm = new int[permutationLength];
        for (int i = 0; i < permutationLength; i++)
        {
            // The x component and y component are [-1, 1)
            float x = (float)(r.NextDouble() * 2 - 1);
            float y = (float)(r.NextDouble() * 2 - 1);
            gradients[i] = new(x, y);
            perm[i] = i;
        }
        for (int n = permutationLength; n > 1; n--)
        {
            int k = r.Next(n);
            int temp = perm[n - 1];
            perm[n - 1] = perm[k];
            perm[k] = temp;
        }
    }

    public float Value(float x, float y)
    {
        // Coordinates of the bottom left corner of the tile
        int x0 = (int)x;
        int y0 = (int)y;
        // Coordinates of the top right corner of the tile
        int x1 = (x0 + 1);
        int y1 = (y0 + 1);
        // Position within the tile
        float xf = x - x0;
        float yf = y - y0;

        // Gradients at the four corners of the tile, with the x coordinate hashed (teleported)
        // to avoid correlation
        Vector2 g00 = gradients[(perm[x0 % perm.Length] + y0) % gradients.Length];
        Vector2 g10 = gradients[(perm[x1 % perm.Length] + y0) % gradients.Length];
        Vector2 g11 = gradients[(perm[x1 % perm.Length] + y1) % gradients.Length];
        Vector2 g01 = gradients[(perm[x0 % perm.Length] + y1) % gradients.Length];

        // Displacements of point (xf, yf) from the four corners (0, 0), (0, 1), (1, 0), (1, 1)
        Vector2 d00 = new(xf, yf);
        Vector2 d01 = new(xf, yf - 1);
        Vector2 d10 = new(xf - 1, yf);
        Vector2 d11 = new(xf - 1, yf - 1);

        float in00 = Vector2.Dot(g00, d00);
        float in01 = Vector2.Dot(g01, d01);
        float in10 = Vector2.Dot(g10, d10);
        float in11 = Vector2.Dot(g11, d11);

        return Mathf.Lerp(Mathf.Lerp(in00, in10, Fade(xf)),
                          Mathf.Lerp(in01, in11, Fade(xf)), Fade(yf)) + 0.5f;

    }

    // Fade function to create the S-curve by Ken Perlin.
    private static float Fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }
}
