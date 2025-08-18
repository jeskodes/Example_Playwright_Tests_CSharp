using Microsoft.Playwright;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

// Image comparison failed by dimensions - 1 px out so need to refine to allow 
// for minor size differences.
namespace TestFramework.Core.Tests.Helper
{
    public class ImagesHelper(IPage page)
    {
        private readonly IPage page = page;

        /// <summary>
        /// Takes a screenshot of a chart element if baseline doesn't exist.
        /// </summary>
        /// <param name="chartName">Chart name (e.g., "results-pie").</param>
        /// <param name="pageFolder">Subfolder for baselines (e.g., "ReportsSummary").</param>
        /// <param name="seriesLocatorCss">CSS selector for chart series.</param>
        /// <param name="chartType">Chart type for logging (e.g., "Pie", "Bar").</param>
        public async Task TakeChartScreenshotAsync(string chartName, string pageFolder, string seriesLocatorCss, string chartType)
        {
            var baselinePath = Path.Combine("Data", "Images", "Baselines", $"{pageFolder}Charts", $"{chartName}.png");
            
            if (File.Exists(baselinePath))
            {
                Console.WriteLine($"Baseline exists, skipping {chartType} chart screenshot");
                return;
            }

            // Create directory if needed
            var directory = Path.GetDirectoryName(baselinePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Find chart wrapper containing the series
            var seriesLocator = page.Locator(seriesLocatorCss);
            var chartLocator = page.Locator(".highcharts-wrapper").Filter(new LocatorFilterOptions
            {
                Has = seriesLocator
            });

            try
            {
                await chartLocator.ScreenshotAsync(new() { Path = baselinePath });
                Console.WriteLine($"Baseline created for {chartType} chart: {baselinePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Screenshot failed for {chartType} chart: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Takes a screenshot of a pie chart.
        /// </summary>
        public async Task TakePieChartScreenshotAsync(string chartName, string pageFolder)
        {
            await TakeChartScreenshotAsync(chartName, pageFolder, ".highcharts-pie-series", "Pie");
        }

        /// <summary>
        /// Takes a screenshot of a bar chart.
        /// </summary>
        public async Task TakeBarChartScreenshotAsync(string chartName, string pageFolder)
        {
            await TakeChartScreenshotAsync(chartName, pageFolder, ".highcharts-column-series", "Bar");
        }

        /// <summary>
        /// Compares current chart with baseline image.
        /// </summary>
        /// <param name="chartName">Chart name.</param>
        /// <param name="pageFolder">Baseline folder.</param>
        /// <param name="chartLocator">Current chart element.</param>
        /// <param name="pixelDifferenceThreshold">Max allowed difference (default 2%).</param>
        /// <returns>True if images match within threshold.</returns>
        public async Task<bool> CompareWithBaseline(string chartName, string pageFolder, ILocator chartLocator, double pixelDifferenceThreshold = 0.02)
        {
            var baselinePath = Path.Combine("Data", "Images", "Baselines", $"{pageFolder}Charts", $"{chartName}.png");
            var diffPath = Path.Combine("Data", "Images", "Diffs", $"{pageFolder}Charts", $"{chartName}_diff.png");

            if (!File.Exists(baselinePath))
            {
                Console.WriteLine($"Baseline not found: {baselinePath}");
                return false;
            }

            var currentScreenshotBytes = await chartLocator.ScreenshotAsync();
            return await CompareImages(baselinePath, currentScreenshotBytes, diffPath, pixelDifferenceThreshold);
        }

        /// <summary>
        /// Performs pixel-by-pixel image comparison.
        /// </summary>
        private async Task<bool> CompareImages(string baselinePath, byte[] currentImageBytes, string diffPath, double pixelDifferenceThreshold)
        {
            try
            {
                using var baselineImage = await Image.LoadAsync<Rgba32>(baselinePath);
                using var currentImage = Image.Load<Rgba32>(currentImageBytes);

                // Create diff directory if needed
                var diffDirectory = Path.GetDirectoryName(diffPath);
                if (!Directory.Exists(diffDirectory))
                {
                    Directory.CreateDirectory(diffDirectory);
                }

                // Check dimensions are within tolerance (3px)
                int widthDiff = Math.Abs(baselineImage.Width - currentImage.Width);
                int heightDiff = Math.Abs(baselineImage.Height - currentImage.Height);
                const int dimensionTolerance = 3;

                if (widthDiff > dimensionTolerance || heightDiff > dimensionTolerance)
                {
                    Console.WriteLine($"Dimensions exceed tolerance. Baseline: {baselineImage.Width}x{baselineImage.Height}, Current: {currentImage.Width}x{currentImage.Height}");
                    await currentImage.SaveAsync(Path.ChangeExtension(diffPath, "_current.png"));
                    return false;
                }

                // Compare pixels
                int diffPixels = 0;
                using var diffImage = new Image<Rgba32>(baselineImage.Width, baselineImage.Height);

                for (int y = 0; y < baselineImage.Height; y++)
                {
                    for (int x = 0; x < baselineImage.Width; x++)
                    {
                        var baselinePixel = baselineImage[x, y];
                        var currentPixel = currentImage[x, y];

                        if (baselinePixel != currentPixel)
                        {
                            diffPixels++;
                            diffImage[x, y] = Color.Red;
                        }
                        else
                        {
                            diffImage[x, y] = baselinePixel;
                        }
                    }
                }

                // Calculate difference percentage
                double totalPixels = baselineImage.Width * baselineImage.Height;
                double differencePercentage = (double)diffPixels / totalPixels;

                if (differencePercentage > pixelDifferenceThreshold)
                {
                    Console.WriteLine($"Images differ by {differencePercentage:P2} (threshold: {pixelDifferenceThreshold:P2})");
                    await diffImage.SaveAsync(diffPath);
                    return false;
                }
                else
                {
                    Console.WriteLine($"Images match (difference: {differencePercentage:P2})");
                    if (File.Exists(diffPath)) File.Delete(diffPath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Image comparison failed: {ex.Message}");
                return false;
            }
        }
    }
}
