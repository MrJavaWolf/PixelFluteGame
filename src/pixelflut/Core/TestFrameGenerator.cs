﻿using System.Diagnostics;
using System.Drawing;

namespace PixelFlut.Core;

public class TestFrameGenerator
{
    private readonly IPixelFlutScreenProtocol screenProtocol;
    private readonly ILogger<TestFrameGenerator> logger;
    private readonly PixelFlutScreenConfiguration screenConfiguration;
    private List<PixelBuffer> frame = new();

    public TestFrameGenerator(
        IPixelFlutScreenProtocol screenProtocol,
        ILogger<TestFrameGenerator> logger,
        PixelFlutScreenConfiguration screenConfiguration)
    {
        this.screenProtocol = screenProtocol;
        this.logger = logger;
        this.screenConfiguration = screenConfiguration;
    }

    public void Startup()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        logger.LogInformation("Creates pixel buffer for the test image...");
        PixelBuffer buffer = new PixelBuffer(screenConfiguration.ResultionY * screenConfiguration.ResultionX, screenProtocol);
        frame.Add(buffer);
        logger.LogInformation($"Pixel buffer for the test image is ready, time took to create: {sw.ElapsedMilliseconds} ms");
    }

    public List<PixelBuffer> Generate(GameTime time)
    {
        PixelBuffer buffer = frame[0];
        int pixelNumber = 0;

        for (int y = 0; y < screenConfiguration.ResultionY; y++)
            for (int x = 0; x < screenConfiguration.ResultionX; x++)
            {
                var c = ColorFromHSV(
                    (x + y + time.TotalTime.TotalSeconds * 100) * 0.3 % 360,
                    1,
                    1);
                buffer.SetPixel(
                    pixelNumber,
                    x,
                    y,
                    c.R,
                    c.G,
                    c.B,
                    255);
                pixelNumber++;
            }
        return frame;
    }

    public List<PixelBuffer> GenerateBlackFrame()
    {
        PixelBuffer buffer = frame[0];
        int pixelNumber = 0;
        for (int y = 0; y < screenConfiguration.ResultionY; y++)
            for (int x = 0; x < screenConfiguration.ResultionX; x++)
            {
                buffer.SetPixel(
                    pixelNumber,
                    x,
                    y,
                    0,
                    0,
                    0,
                    255);
                pixelNumber++;
            }
        return frame;
    }

    public static Color ColorFromHSV(double hue, double saturation, double value)
    {
        int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
        double f = hue / 60 - Math.Floor(hue / 60);

        value = value * 255;
        int v = Convert.ToInt32(value);
        int p = Convert.ToInt32(value * (1 - saturation));
        int q = Convert.ToInt32(value * (1 - f * saturation));
        int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

        if (hi == 0)
            return Color.FromArgb(255, v, t, p);
        else if (hi == 1)
            return Color.FromArgb(255, q, v, p);
        else if (hi == 2)
            return Color.FromArgb(255, p, v, t);
        else if (hi == 3)
            return Color.FromArgb(255, p, q, v);
        else if (hi == 4)
            return Color.FromArgb(255, t, p, v);
        else
            return Color.FromArgb(255, v, p, q);
    }
}
