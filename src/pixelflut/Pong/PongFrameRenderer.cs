﻿using PixelFlut.Core;
using System.Drawing;

namespace PixelFlut.Pong;

public static class PongFrameRenderer
{
    private static readonly Color BallColor = Color.White;
    private static readonly Color BackgroundColor = Color.Black;
    public static int CalculatePixelsInFrame(
        PongConfiguration pongConfig,
        PongGameState gameState)
    {
        // Draw the ball
        int pixelsDrawn = 0;
        pixelsDrawn += DrawBall(pongConfig, gameState, pixelsDrawn, null);

        // Draw the players
        pixelsDrawn += DrawPlayer(
            (int)gameState.Player1Position.X,
            (int)gameState.Player1Position.Y,
            pongConfig,
            pixelsDrawn,
            null);
        pixelsDrawn += DrawPlayer(
            (int)gameState.Player2Position.X,
            (int)gameState.Player2Position.Y,
            pongConfig,
            pixelsDrawn,
            null);
        return pixelsDrawn;
    }

    public static int DrawFrame(
        PongConfiguration pongConfig,
        PongGameState gameState,
        PixelBuffer buffer,
        GameTime time)
    {
        // Draw the ball
        int pixelsDrawn = 0;
        pixelsDrawn += DrawBall(pongConfig, gameState, pixelsDrawn, buffer);

        // Draw the players
        pixelsDrawn += DrawPlayer(
            (int)gameState.Player1Position.X + CalculatePlayerBallHitOffset(gameState.Player1LastHitTime, time, gameState) * -1,
            (int)gameState.Player1Position.Y,
            pongConfig,
            pixelsDrawn,
            buffer);
        pixelsDrawn += DrawPlayer(
            (int)gameState.Player2Position.X - pongConfig.PlayerWidth + CalculatePlayerBallHitOffset(gameState.Player2LastHitTime, time, gameState),
            (int)gameState.Player2Position.Y,
            pongConfig,
            pixelsDrawn,
            buffer);

        return pixelsDrawn;
    }
    private static int CalculatePlayerBallHitOffset(
        TimeSpan hitTime,
        GameTime time,
        PongGameState gameState)
    {
        if (hitTime == TimeSpan.Zero) return 0;

        // Data
        TimeSpan pushbackTime = TimeSpan.FromMilliseconds(100);
        TimeSpan recoveryTime = TimeSpan.FromMilliseconds(300);
        int pushBackAmount = 2 * gameState.BallBounces;

        // Check if the effect is over
        if (time.TotalTime > hitTime + pushbackTime + recoveryTime) return 0;

        // Calculation
        if (time.TotalTime < hitTime + pushbackTime)
        {
            // Pushback
            float ratio = (float)((time.TotalTime.TotalSeconds - hitTime.TotalSeconds) / pushbackTime.TotalSeconds);
            float amount = MathHelper.RemapRange(ratio, 0, 1, 0, pushBackAmount);
            return (int)amount;
        }
        else
        {
            // Recovery
            float ratio = (float)((time.TotalTime.TotalSeconds - pushbackTime.TotalSeconds - hitTime.TotalSeconds) / recoveryTime.TotalSeconds);
            float amount = MathHelper.RemapRange(ratio, 0, 1, pushBackAmount, 0);
            return (int)amount;
        }
    }

    private static int DrawBall(
        PongConfiguration pongConfig,
        PongGameState gameState,
        int pixelOffset,
        PixelBuffer? buffer)
    {
        int numberOfPixels = 0;
        for (int x = 0; x < pongConfig.BallRadius * 2 + pongConfig.BallBorder * 2; x++)
        {
            for (int y = 0; y < pongConfig.BallRadius * 2 + pongConfig.BallBorder * 2; y++)
            {
                int ballPixelX = (int)gameState.BallPosition.X - pongConfig.BallBorder - pongConfig.BallRadius + x;
                int ballPixelY = (int)gameState.BallPosition.Y - pongConfig.BallBorder - pongConfig.BallRadius + y;
                if (x < pongConfig.BallBorder ||
                    y < pongConfig.BallBorder ||
                    x > pongConfig.BallRadius * 2 + pongConfig.BallBorder ||
                    y > pongConfig.BallRadius * 2 + pongConfig.BallBorder)
                {
                    buffer?.SetPixel(pixelOffset + numberOfPixels, ballPixelX, ballPixelY, BackgroundColor);
                }
                else
                {
                    buffer?.SetPixel(pixelOffset + numberOfPixels, ballPixelX, ballPixelY, BallColor);
                }
                numberOfPixels++;
            }
        }
        return numberOfPixels;
    }

    private static int DrawPlayer(
        int playerPositionX,
        int playerPositionY,
        PongConfiguration pongConfig,
        int pixelOffset,
        PixelBuffer? buffer)
    {
        int numberOfPixels = 0;

        // Draw playerborder above the player, this will make it easiere to see the player
        for (int x = playerPositionX; x < playerPositionX + pongConfig.PlayerWidth; x++)
        {
            for (int y = playerPositionY - pongConfig.PlayerBorder; y < playerPositionY; y++)
            {
                buffer?.SetPixel(pixelOffset + numberOfPixels, x, y, BackgroundColor);
                numberOfPixels++;
            }
        }

        // Draw playerborder below the player, this will make it easiere to see the player
        for (int x = playerPositionX; x < playerPositionX + pongConfig.PlayerWidth; x++)
        {
            int yStart = playerPositionY + pongConfig.PlayerHeight + 1;
            int yEnd = playerPositionY + pongConfig.PlayerHeight + 1 + pongConfig.PlayerBorder;
            for (int y = yStart; y < yEnd; y++)
            {
                buffer?.SetPixel(pixelOffset + numberOfPixels, x, y, BackgroundColor);
                numberOfPixels++;
            }
        }

        // Draw playerborder to the left of the player, this will make it easiere to see the player
        for (int x = playerPositionX - pongConfig.PlayerBorder; x < playerPositionX; x++)
        {
            for (int y = playerPositionY; y < playerPositionY + pongConfig.PlayerHeight; y++)
            {
                buffer?.SetPixel(pixelOffset + numberOfPixels, x, y, BackgroundColor);
                numberOfPixels++;
            }
        }

        // Draw playerborder to the right of the player, this will make it easiere to see the player
        for (int x = playerPositionX + pongConfig.PlayerWidth; x < playerPositionX + pongConfig.PlayerWidth + pongConfig.PlayerBorder; x++)
        {
            for (int y = playerPositionY; y < playerPositionY + pongConfig.PlayerHeight; y++)
            {
                buffer?.SetPixel(pixelOffset + numberOfPixels, x, y, BackgroundColor);
                numberOfPixels++;
            }
        }

        // Draw player
        for (int x = playerPositionX; x < playerPositionX + pongConfig.PlayerWidth; x++)
        {
            for (int y = playerPositionY; y < playerPositionY + pongConfig.PlayerHeight; y++)
            {
                Color c;
                // Top stripe
                if (y >= playerPositionY + 5 && y <= playerPositionY + 10)
                    c = Color.Black;

                // Bottom stripe
                else if (y >= playerPositionY + pongConfig.PlayerHeight - 10 && y <= playerPositionY + pongConfig.PlayerHeight - 5)
                    c = Color.Black;

                //// A center white/black gradient inside the player
                //else if (x > playerPositionX + 2 &&
                //    x < playerPositionX + pongConfig.PlayerWidth - 2 &&
                //    y > playerPositionY + 10 &&
                //    y <= playerPositionY + pongConfig.PlayerHeight - 10)
                //{
                //    Color startColor = Color.White;
                //    Color endColor = Color.Black;
                //    int localY = (y - playerPositionY);
                //    float middlePoint = pongConfig.PlayerHeight / 2.0f;
                //    float amount = localY < middlePoint ?
                //        MathHelper.RemapRange(localY, 0, middlePoint, 0, 1) :
                //        (1 - MathHelper.RemapRange(localY, middlePoint, pongConfig.PlayerHeight, 0, 1));
                //    c = startColor.Lerp(endColor, amount);
                //}

                // Normal color
                else c = Color.White;

                buffer?.SetPixel(
                      pixelOffset + numberOfPixels,
                      x,
                      y,
                      c);
                numberOfPixels++;
            }
        }
        return numberOfPixels;
    }
}
