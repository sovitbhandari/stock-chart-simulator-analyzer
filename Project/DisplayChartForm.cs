
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.AxHost;

namespace Project3
{
    public partial class ChartDisplayForm : Form
    {
        // ──────────── fields ────────────
        private BindingList<SmartCandleStick> BindingCandleSticks { get; set; } = new BindingList<SmartCandleStick>();
        private List<CandleStick> TempList;
        private string FileName;
        #region rubber-band selection fields
        private bool rbIsDragging = false;
        private Point rbStartPx;
        private Point rbEndPx;
        private Rectangle rbRect = Rectangle.Empty;
        private bool isOutOfBounds = false;
        private const int RB_HANDLE = 7;
        private int selStartIdx = -1;
        private int selEndIdx = -1;
        #endregion
        #region simulator fields
        private Timer simulationTimer;
        private Button btnStartStop;
        private Button btnIncrease;
        private Button btnDecrease;
        private HScrollBar scrollMinPrice;
        private HScrollBar scrollMaxPrice;
        private HScrollBar scrollStepSize;
        private bool isSimulating = false;
        private double minPricePercentage = 0; // 0% of wave height
        private double maxPricePercentage = 100; // 100% of wave height
        private double priceStep = 0.1; // $0.1 step size
        private int simulationDirection = 1; // 1 for increasing, -1 for decreasing
        private double waveMinPrice;
        private double waveMaxPrice;

        private int startY = 50;
        // New fields for the fixed rectangle and moving price line
        private Rectangle fixedRbRect;
        private double currentSimulationPrice;
        private bool isUpWave;
        #endregion

        // ─────────── constructors ───────
        public ChartDisplayForm(string file, List<CandleStick> candlesticks, DateTime from)
        {
            InitializeComponent();
            InitializeSimulatorControls();
            InitScrollBars();
            dateTimePickerEndDate.Value = DateTime.Now;
            chart_stockData.MouseDown += Chart_MouseDown;
            chart_stockData.MouseMove += Chart_MouseMove;
            chart_stockData.MouseUp += Chart_MouseUp;
            chart_stockData.Paint += Chart_Paint;
            LoadDefaultStockData();
        }

        public ChartDisplayForm(string filename,
                               List<CandleStick> candlesticks,
                               DateTimePicker start,
                               DateTimePicker end)
        {
            try
            {
                InitializeComponent();
                InitializeSimulatorControls();
                TempList = new List<CandleStick>(candlesticks);
                FileName = filename;
                dateTimePickerStartDate.Value = start.Value;
                dateTimePickerEndDate.Value = end.Value;

                GetCandlesticksInDateRange(start.Value, end.Value);

                if (!BindingCandleSticks.Any())
                    MessageBox.Show($"No data in {start.Value:MM/dd/yyyy} – {end.Value:MM/dd/yyyy}");

                ConfigureChart();
                chart_stockData.MouseDown += Chart_MouseDown;
                chart_stockData.MouseMove += Chart_MouseMove;
                chart_stockData.MouseUp += Chart_MouseUp;
                chart_stockData.Paint += Chart_Paint;
                UpdateChart();
                DetectWaves();
                UpdatePeaksAndValleys();

                chart_stockData.Titles.Clear();
                chart_stockData.Titles.Add(Path.GetFileNameWithoutExtension(FileName));
                chart_stockData.Titles[0].Font = new Font("Microsoft Sans Serif", 14.2f);
                chart_stockData.Legends.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}");
                Close();
            }
        }

        // ─────────── simulator setup ────────
        private void InitializeSimulatorControls()
        {
            // Timer
            simulationTimer = new Timer
            {
                Interval = 100 // 100ms for smooth animation
            };
            simulationTimer.Tick += SimulationTimer_Tick;

            // Start/Stop button
            btnStartStop = new Button
            {
                Location = new Point(10, 10),
                Size = new Size(60, 30),
                Text = "Start"
            };
            btnStartStop.Click += BtnStartStop_Click;
            Controls.Add(btnStartStop);

            // Increase button
            btnIncrease = new Button
            {
                Location = new Point(75, 10),
                Size = new Size(30, 30),
                Text = "+"
            };
            btnIncrease.Click += BtnIncrease_Click;
            Controls.Add(btnIncrease);

            // Decrease button
            btnDecrease = new Button
            {
                Location = new Point(110, 10),
                Size = new Size(30, 30),
                Text = "-"
            };
            btnDecrease.Click += BtnDecrease_Click;
            Controls.Add(btnDecrease);

            startY = 50;
            int labelWidth = 80;
            int scrollBarWidth = 150;
            int valueLabelWidth = 50;
            int verticalSpacing = 30;

            // Min Price Scroll Bar, Label, and Value Display
            Label lblMinPrice = new Label
            {
                Location = new Point(145, startY),
                Size = new Size(labelWidth, 20),
                Text = "Min Price:"
            };
            Controls.Add(lblMinPrice);

            scrollMinPrice = new HScrollBar
            {
                Location = new Point(145 + labelWidth, startY),
                Size = new Size(scrollBarWidth, 20),
                Minimum = 0,
                Maximum = 100,
                Value = 0
            };
            scrollMinPrice.Scroll += ScrollMinPrice_Scroll;
            Controls.Add(scrollMinPrice);

            Label lblMinPriceValue = new Label
            {
                Location = new Point(145 + labelWidth + scrollBarWidth + 5, startY),
                Size = new Size(valueLabelWidth, 20),
                Text = "0%",
                Name = "lblMinPriceValue"
            };
            Controls.Add(lblMinPriceValue);

            // Max Price Scroll Bar, Label, and Value Display
            Label lblMaxPrice = new Label
            {
                Location = new Point(145, startY + verticalSpacing),
                Size = new Size(labelWidth, 20),
                Text = "Max Price:"
            };
            Controls.Add(lblMaxPrice);

            scrollMaxPrice = new HScrollBar
            {
                Location = new Point(145 + labelWidth, startY + verticalSpacing),
                Size = new Size(scrollBarWidth, 20),
                Minimum = 0,
                Maximum = 100,
                Value = 100
            };
            scrollMaxPrice.Scroll += ScrollMaxPrice_Scroll;
            Controls.Add(scrollMaxPrice);

            Label lblMaxPriceValue = new Label
            {
                Location = new Point(145 + labelWidth + scrollBarWidth + 5, startY + verticalSpacing),
                Size = new Size(valueLabelWidth, 20),
                Text = "100%",
                Name = "lblMaxPriceValue"
            };
            Controls.Add(lblMaxPriceValue);

            // Step Size Scroll Bar, Label, and Value Display
            Label lblStepSize = new Label
            {
                Location = new Point(145, startY + 2 * verticalSpacing),
                Size = new Size(labelWidth, 20),
                Text = "Step Size:"
            };
            Controls.Add(lblStepSize);

            scrollStepSize = new HScrollBar
            {
                Location = new Point(145 + labelWidth, startY + 2 * verticalSpacing),
                Size = new Size(scrollBarWidth, 20),
                Minimum = 1,
                Maximum = 100,
                Value = 10
            };
            scrollStepSize.Scroll += ScrollStepSize_Scroll;
            Controls.Add(scrollStepSize);

            Label lblStepSizeValue = new Label
            {
                Location = new Point(145 + labelWidth + scrollBarWidth + 5, startY + 2 * verticalSpacing),
                Size = new Size(valueLabelWidth, 20),
                Text = "0.10",
                Name = "lblStepSizeValue"
            };
            Controls.Add(lblStepSizeValue);
        }
        private void InitScrollBars()
        {
            // WinForms quirk: to reach Maximum you must
            // set Maximum += LargeChange-1
            const int FULL = 100;

            void fix(HScrollBar sb, int min, int def)
            {
                sb.Minimum = min;
                sb.LargeChange = 1;
                sb.SmallChange = 1;
                sb.Maximum = FULL + sb.LargeChange - 1;
                sb.Value = def;
            }

            fix(scrollMinPrice, 0, 0);   //  0 %
            fix(scrollMaxPrice, 0, 100);   // 100 %
            fix(scrollStepSize, 1, 10);   // 0.10 $
        }

        private void LoadDefaultStockData()
        {
            try
            {
                // Load ABBV, daily, February 2021
                FileName = "ABBV.csv";
                dateTimePickerStartDate.Value = new DateTime(2021, 2, 1);
                dateTimePickerEndDate.Value = new DateTime(2021, 2, 28);

                TempList = new List<CandleStick>();
                string path = Path.Combine(Application.StartupPath, "Data", "ABBV.csv");
                if (File.Exists(path))
                {
                    var lines = File.ReadAllLines(path);
                    foreach (var line in lines.Skip(1)) // Skip header
                    {
                        var cs = new CandleStick(line);
                        TempList.Add(cs);
                    }
                }
                else
                {
                    MessageBox.Show("ABBV.csv not found in Data folder.");
                    return;
                }

                GetCandlesticksInDateRange(dateTimePickerStartDate.Value, dateTimePickerEndDate.Value);
                if (!BindingCandleSticks.Any())
                {
                    MessageBox.Show("No ABBV data for February 2021.");
                    return;
                }

                ConfigureChart();
                UpdateChart();
                DetectWaves();
                UpdatePeaksAndValleys();
                chart_stockData.Titles.Clear();
                chart_stockData.Titles.Add("ABBV");
                chart_stockData.Titles[0].Font = new Font("Microsoft Sans Serif", 14.2f);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading ABBV data: {ex.Message}");
            }
        }

        // ─────────── simulator event handlers ────────
        private void BtnStartStop_Click(object sender, EventArgs e)
        {
            if (!isSimulating)
            {
                // Start simulation
                if (rbRect == Rectangle.Empty && (comboBox_upWaves.SelectedItem == null && comboBox_downWaves.SelectedItem == null))
                {
                    MessageBox.Show("Select a wave or start rubber-banding first.");
                    return;
                }

                isSimulating = true;
                btnStartStop.Text = "Stop";
                btnIncrease.Enabled = false;
                btnDecrease.Enabled = false;

                // Freeze the rubber-band rectangle
                fixedRbRect = rbRect;

                // Determine wave direction
                var area = chart_stockData.ChartAreas["area_OHLC"];
                double startPrice = area.AxisY.PixelPositionToValue(rbStartPx.Y);
                double endPrice = area.AxisY.PixelPositionToValue(rbEndPx.Y);
                isUpWave = startPrice > endPrice;

                // Initialize wave price range
                ComputeWavePriceRange();

                // Initialize price line position
                currentSimulationPrice = startPrice;
                simulationDirection = isUpWave ? -1 : 1;

                simulationTimer.Start();
            }
            else
            {
                // Stop simulation
                StopSimulation();
            }
        }

        private void BtnIncrease_Click(object sender, EventArgs e)
        {
            if (rbRect == Rectangle.Empty)
            {
                MessageBox.Show("Select a wave or start rubber-banding first.");
                return;
            }
            AdjustWavePrice(priceStep);
        }

        private void BtnDecrease_Click(object sender, EventArgs e)
        {
            if (rbRect == Rectangle.Empty)
            {
                MessageBox.Show("Select a wave or start rubber-banding first.");
                return;
            }
            AdjustWavePrice(-priceStep);
        }

        private void SimulationTimer_Tick(object sender, EventArgs e)
        {
            currentSimulationPrice += simulationDirection * priceStep;

            if (currentSimulationPrice >= waveMaxPrice || currentSimulationPrice <= waveMinPrice)
            {
                simulationDirection *= -1;
                currentSimulationPrice = Math.Max(waveMinPrice,
                    Math.Min(currentSimulationPrice, waveMaxPrice));
            }

            chart_stockData.Invalidate();
        }

        private void ScrollMinPrice_Scroll(object sender, ScrollEventArgs e)
        {
            minPricePercentage = scrollMinPrice.Value;

            // Update Min Price VALUE label
            var minPriceValueLabel = Controls.Find("lblMinPriceValue", true).FirstOrDefault() as Label;
            if (minPriceValueLabel != null)
                minPriceValueLabel.Text = $"{minPricePercentage}%";

            if (minPricePercentage > maxPricePercentage)
            {
                maxPricePercentage = minPricePercentage;
                scrollMaxPrice.Value = (int)maxPricePercentage;

                // Update Max Price VALUE label
                var maxPriceValueLabel = Controls.Find("lblMaxPriceValue", true).FirstOrDefault() as Label;
                if (maxPriceValueLabel != null)
                    maxPriceValueLabel.Text = $"{maxPricePercentage}%";
            }

            RecomputeEnvelope();
        }

        private void ScrollMaxPrice_Scroll(object sender, ScrollEventArgs e)
        {
            maxPricePercentage = scrollMaxPrice.Value;

            // Update Max Price VALUE label
            var maxPriceValueLabel = Controls.Find("lblMaxPriceValue", true).FirstOrDefault() as Label;
            if (maxPriceValueLabel != null)
                maxPriceValueLabel.Text = $"{maxPricePercentage}%";

            if (maxPricePercentage < minPricePercentage)
            {
                minPricePercentage = maxPricePercentage;
                scrollMinPrice.Value = (int)minPricePercentage;

                // Update Min Price VALUE label
                var minPriceValueLabel = Controls.Find("lblMinPriceValue", true).FirstOrDefault() as Label;
                if (minPriceValueLabel != null)
                    minPriceValueLabel.Text = $"{minPricePercentage}%";
            }

            RecomputeEnvelope();
        }

        private void ScrollStepSize_Scroll(object sender, ScrollEventArgs e)
        {
            priceStep = scrollStepSize.Value / 100.0;

            // Update Step Size VALUE label
            var stepSizeValueLabel = Controls.Find("lblStepSizeValue", true).FirstOrDefault() as Label;
            if (stepSizeValueLabel != null)
                stepSizeValueLabel.Text = priceStep.ToString("F2");
        }


        // ─────────── simulator helpers ────────
        private void ComputeWavePriceRange()
        {
            var area = chart_stockData.ChartAreas["area_OHLC"];
            double startPrice = area.AxisY.PixelPositionToValue(rbStartPx.Y);
            double endPrice = area.AxisY.PixelPositionToValue(rbEndPx.Y);
            double high = Math.Max(startPrice, endPrice);
            double low = Math.Min(startPrice, endPrice);
            double range = high - low;

            waveMinPrice = low + (minPricePercentage / 100.0) * range;
            waveMaxPrice = low + (maxPricePercentage / 100.0) * range;
        }
        private void RecomputeEnvelope()
        {
            ComputeWavePriceRange();

            // Clamp price and auto-reverse direction if at boundaries
            currentSimulationPrice = Math.Max(waveMinPrice, Math.Min(currentSimulationPrice, waveMaxPrice));

            if (currentSimulationPrice >= waveMaxPrice)
            {
                simulationDirection = -1; // Reverse to downward movement
            }
            else if (currentSimulationPrice <= waveMinPrice)
            {
                simulationDirection = 1; // Reverse to upward movement
            }

            chart_stockData.Invalidate();
        }

        private void AdjustWavePrice(double delta)
        {
            if (!isSimulating)
            {
                // If not simulating, initialize the current price if needed
                if (currentSimulationPrice == 0 && rbRect != Rectangle.Empty)
                {
                    var area = chart_stockData.ChartAreas["area_OHLC"];
                    currentSimulationPrice = area.AxisY.PixelPositionToValue(rbEndPx.Y);
                }
            }

            currentSimulationPrice += delta;

            if (currentSimulationPrice > waveMaxPrice) currentSimulationPrice = waveMaxPrice;
            if (currentSimulationPrice < waveMinPrice) currentSimulationPrice = waveMinPrice;

            chart_stockData.Invalidate();
        }

        private void StopSimulation()
        {
            isSimulating = false;
            simulationTimer.Stop();
            btnStartStop.Text = "Start";
            btnIncrease.Enabled = true;
            btnDecrease.Enabled = true;
            scrollMinPrice.Enabled = scrollMaxPrice.Enabled = scrollStepSize.Enabled = true;
            fixedRbRect = Rectangle.Empty;
        }

        private void UpdateWaveDisplay()
        {
            if (!isSimulating)
            {
                // Original behavior when not simulating
                rbRect = new Rectangle(
                    Math.Min(rbStartPx.X, rbEndPx.X),
                    Math.Min(rbStartPx.Y, rbEndPx.Y),
                    Math.Abs(rbEndPx.X - rbStartPx.X),
                    Math.Abs(rbEndPx.Y - rbStartPx.Y));
            }

            // Update confirmations and yellow dots
            var yellowDots = chart_stockData.Annotations
                .Where(a => a.Tag != null && a.Tag.ToString() == "YellowDot")
                .ToList();
            foreach (var dot in yellowDots)
            {
                chart_stockData.Annotations.Remove(dot);
            }

            int dragDistance = Math.Abs(rbEndPx.X - rbStartPx.X);
            int confirmationInterval = 50;
            int confirmationCount = dragDistance / confirmationInterval;
            label1.Text = $"Confirmations: {confirmationCount}";

            for (int i = 1; i <= confirmationCount; i++)
            {
                int xPos = rbStartPx.X + (i * confirmationInterval * (rbEndPx.X >= rbStartPx.X ? 1 : -1));
                int yPos = rbStartPx.Y;
                var dot = new EllipseAnnotation
                {
                    X = xPos,
                    Y = yPos,
                    Width = 10,
                    Height = 10,
                    BackColor = Color.Yellow,
                    LineColor = Color.Black,
                    Tag = "YellowDot"
                };
                chart_stockData.Annotations.Add(dot);
            }

            //chart_stockData.Invalidate();
        }

        // ─────────── wave selection ────────
        private void comboBox_upWaves_SelectedIndexChanged(object s, EventArgs e)
        {
            if (comboBox_upWaves.SelectedItem is Wave w)
            {
                // Find the actual high and low within the wave's range
                decimal waveHi = BindingCandleSticks.Skip(w.StartIndex).Take(w.EndIndex - w.StartIndex + 1).Max(cs => cs.High);
                decimal waveLo = BindingCandleSticks.Skip(w.StartIndex).Take(w.EndIndex - w.StartIndex + 1).Min(cs => cs.Low);

                var area = chart_stockData.ChartAreas["area_OHLC"];
                rbStartPx = new Point(
                    (int)area.AxisX.ValueToPixelPosition(w.StartIndex + 0.5),
                    (int)area.AxisY.ValueToPixelPosition((double)waveHi));
                rbEndPx = new Point(
                    (int)area.AxisX.ValueToPixelPosition(w.EndIndex + 0.5),
                    (int)area.AxisY.ValueToPixelPosition((double)waveLo));
                DrawWave(w);
                UpdateWaveDisplay();
                UpdatePeaksAndValleys();
            }
        }


        private void comboBox_downWaves_SelectedIndexChanged(object s, EventArgs e)
        {
            if (comboBox_downWaves.SelectedItem is Wave w)
            {
                // Find the actual high and low within the wave's range
                decimal waveHi = BindingCandleSticks.Skip(w.StartIndex).Take(w.EndIndex - w.StartIndex + 1).Max(cs => cs.High);
                decimal waveLo = BindingCandleSticks.Skip(w.StartIndex).Take(w.EndIndex - w.StartIndex + 1).Min(cs => cs.Low);

                var area = chart_stockData.ChartAreas["area_OHLC"];
                rbStartPx = new Point(
                    (int)area.AxisX.ValueToPixelPosition(w.StartIndex + 0.5),
                    (int)area.AxisY.ValueToPixelPosition((double)waveLo));
                rbEndPx = new Point(
                    (int)area.AxisX.ValueToPixelPosition(w.EndIndex + 0.5),
                    (int)area.AxisY.ValueToPixelPosition((double)waveHi));
                DrawWave(w);
                UpdateWaveDisplay();
                UpdatePeaksAndValleys();
            }
        }

        // ─────────── chart setup ────────
        private void ConfigureChart()
        {
            chart_stockData.Series["series_OHLC"].XValueType = ChartValueType.DateTime;
            chart_stockData.Series["series_Volume"].XValueType = ChartValueType.DateTime;
            chart_stockData.Series["series_OHLC"].IsXValueIndexed = true;
            chart_stockData.Series["series_Volume"].IsXValueIndexed = true;

            var oh = chart_stockData.ChartAreas["area_OHLC"];
            oh.AxisX.LabelStyle.Format = "MM/dd";
            oh.AxisX.IntervalType = DateTimeIntervalType.Auto;
            oh.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;

            chart_stockData.ChartAreas["area_Volume"].AxisY.Minimum = 0;
        }

        private void GetCandlesticksInDateRange(DateTime start, DateTime end)
        {
            const int MAX = 1000;
            BindingCandleSticks.Clear();
            BindingCandleSticks = new BindingList<SmartCandleStick>(
                TempList.Where(cs => cs.Date >= start && cs.Date <= end)
                        .OrderBy(cs => cs.Date)
                        .Take(MAX)
                        .Select(cs => new SmartCandleStick(cs))
                        .ToList());
        }

        private void UpdateChart()
        {
            var ohlc = chart_stockData.Series["series_OHLC"];
            var vol = chart_stockData.Series["series_Volume"];
            ohlc.Points.Clear();
            vol.Points.Clear();

            foreach (var scs in BindingCandleSticks)
            {
                int idx = ohlc.Points.AddY((double)scs.High,
                                           (double)scs.Low,
                                           (double)scs.Open,
                                           (double)scs.Close);
                ohlc.Points[idx].XValue = scs.Date.ToOADate();
                vol.Points.AddXY(scs.Date, scs.Volume);
            }
            NormalizeChart();
        }

        private void NormalizeChart()
        {
            if (BindingCandleSticks.Count == 0) return;

            var inRange = BindingCandleSticks.Where(cs =>
                             cs.Date >= dateTimePickerStartDate.Value &&
                             cs.Date <= dateTimePickerEndDate.Value).ToList();
            if (!inRange.Any()) return;

            decimal minLow = inRange.Min(c => c.Low);
            decimal maxHigh = inRange.Max(c => c.High);
            double ymin = (double)minLow * 0.98;
            double ymax = (double)maxHigh * 1.02;

            var oh = chart_stockData.ChartAreas["area_OHLC"];
            oh.AxisY.Minimum = ymin;
            oh.AxisY.Maximum = ymax;
            oh.AxisY.Interval = Math.Ceiling((ymax - ymin) / 10.0);
            oh.AxisY.LabelStyle.Format = "F2";

            var vol = chart_stockData.ChartAreas["area_Volume"];
            double maxVol = (double)inRange.Max(c => c.Volume);
            vol.AxisY.Maximum = maxVol * 1.1;
            vol.AxisY.Interval = Math.Ceiling(maxVol / 10.0);
        }

        // ─────────── peaks / valleys ────
        private void DetectPeaks()
        {
            // clear old arrows & text
            var oldAnns = chart_stockData.Annotations
                            .Where(a => a.Tag?.ToString() == "PeakText" ||
                                        (a is ArrowAnnotation && a.Tag == null))
                            .ToList();
            foreach (var a in oldAnns)
                chart_stockData.Annotations.Remove(a);

            int m = hScrollBar_margin.Value;
            if (BindingCandleSticks.Count <= 2 * m) return;

            var area = chart_stockData.ChartAreas["area_OHLC"];
            var series = chart_stockData.Series["series_OHLC"];

            for (int i = m; i < BindingCandleSticks.Count - m; i++)
            {
                if (IsPeak(i, m))
                {
                    // arrow
                    AddArrowAnnotation(i,
                        (double)BindingCandleSticks[i].High * 1.01,
                        Color.Red,
                        up: true);

                    // text
                    var dp = series.Points[i];
                    var txt = new TextAnnotation
                    {
                        AxisX = area.AxisX,
                        AxisY = area.AxisY,
                        AnchorDataPoint = dp,
                        AnchorAlignment = ContentAlignment.TopCenter,
                        Text = "Peak Value",
                        Font = new Font("Arial", 8, FontStyle.Bold),
                        ForeColor = Color.Red,
                        Tag = "PeakText"
                    };
                    chart_stockData.Annotations.Add(txt);
                }
            }
        }


        private void ClearPeakValleyArrows()
        {
            var toDelete = chart_stockData.Annotations
                           .OfType<ArrowAnnotation>()
                           .Where(a => a.Tag == null                // old arrows (no tag)
                                    || !a.Tag.ToString()
                                           .Equals("ConfirmationArrow"))
                           .ToList();

            foreach (var a in toDelete)
                chart_stockData.Annotations.Remove(a);
        }

        private void DetectValleys()
        {
            int m = hScrollBar_margin.Value;
            if (BindingCandleSticks.Count <= 2 * m) return;

            for (int i = m; i < BindingCandleSticks.Count - m; i++)
                if (IsValley(i, m))
                    AddArrowAnnotation(i, (double)BindingCandleSticks[i].Low * 0.99,
                                       Color.Green, false);
        }

        private void UpdatePeaksAndValleys()
        {
            ClearPeakValleyArrows();
            DetectPeaks();
            DetectValleys();
        }

        private static bool Greater(decimal a, decimal b) => a > b + 0.0000001m;
        private static bool Less(decimal a, decimal b) => a < b - 0.0000001m;

        private bool IsPeak(int i, int m)
        {
            decimal h = BindingCandleSticks[i].High;
            for (int k = 1; k <= m; k++)
                if (!Greater(h, BindingCandleSticks[i - k].High) ||
                    !Greater(h, BindingCandleSticks[i + k].High))
                    return false;
            return true;
        }

        private bool IsValley(int i, int m)
        {
            decimal l = BindingCandleSticks[i].Low;
            for (int k = 1; k <= m; k++)
                if (!Less(l, BindingCandleSticks[i - k].Low) ||
                    !Less(l, BindingCandleSticks[i + k].Low))
                    return false;
            return true;
        }

        private void AddArrowAnnotation(int idx, double y, Color color, bool up)
        {
            chart_stockData.Annotations.Add(new ArrowAnnotation
            {
                AxisX = chart_stockData.ChartAreas["area_OHLC"].AxisX,
                AxisY = chart_stockData.ChartAreas["area_OHLC"].AxisY,
                AnchorDataPoint = chart_stockData.Series["series_OHLC"].Points[idx],
                LineColor = color,
                Height = up ? -5 : 5,
                Width = 1,
                LineWidth = 1,
                Y = y
            });
        }

        // ─────────── waves & confirmations ─
        private void DetectWaves()
        {
            var peaks = new List<int>();
            var valleys = new List<int>();
            int m = hScrollBar_margin.Value;

            if (BindingCandleSticks.Count <= 2 * m)
            {
                comboBox_upWaves.Items.Clear();
                comboBox_downWaves.Items.Clear();
                return;
            }

            for (int i = m; i < BindingCandleSticks.Count - m; i++)
            {
                if (IsPeak(i, m)) peaks.Add(i);
                if (IsValley(i, m)) valleys.Add(i);
            }

            comboBox_upWaves.Items.Clear();
            comboBox_downWaves.Items.Clear();

            for (int i = 0; i < Math.Min(peaks.Count, valleys.Count); i++)
            {
                if (peaks[i] < valleys[i])
                {
                    if (peaks[i] >= 0 && peaks[i] < BindingCandleSticks.Count && valleys[i] >= 0 && valleys[i] < BindingCandleSticks.Count)
                    {
                        SmartCandleStick peakCandle = (SmartCandleStick)BindingCandleSticks[peaks[i]];
                        SmartCandleStick valleyCandle = (SmartCandleStick)BindingCandleSticks[valleys[i]];
                        string label = $"{peakCandle.Date.ToString("MM/dd/yyyy")} – {valleyCandle.Date.ToString("MM/dd/yyyy")}";
                        comboBox_upWaves.Items.Add(new Wave(peaks[i], valleys[i], label, true));
                    }
                }
            }

            for (int i = 0; i < Math.Min(valleys.Count, peaks.Count - 1); i++)
            {
                if (valleys[i] < peaks[i + 1])
                {
                    if (valleys[i] >= 0 && valleys[i] < BindingCandleSticks.Count && peaks[i + 1] >= 0 && peaks[i + 1] < BindingCandleSticks.Count)
                    {
                        SmartCandleStick valleyCandle = (SmartCandleStick)BindingCandleSticks[valleys[i]];
                        SmartCandleStick peakCandle = (SmartCandleStick)BindingCandleSticks[peaks[i + 1]];
                        string label = $"{valleyCandle.Date.ToString("MM/dd/yyyy")} – {peakCandle.Date.ToString("MM/dd/yyyy")}";
                        comboBox_downWaves.Items.Add(new Wave(valleys[i], peaks[i + 1], label, false));
                    }
                }
            }
        }

        private void AddConfirmationArrow(int idx, double y, bool isLow)
        {
            chart_stockData.Annotations.Add(new ArrowAnnotation
            {
                AxisX = chart_stockData.ChartAreas["area_OHLC"].AxisX,
                AxisY = chart_stockData.ChartAreas["area_OHLC"].AxisY,
                AnchorDataPoint = chart_stockData.Series["series_OHLC"].Points[idx],
                LineColor = Color.Yellow,
                Height = isLow ? 4 : -4,   // ↓ for highs, ↑ for lows
                Width = 1,
                LineWidth = 2,
                Y = y,
                Tag = "ConfirmationArrow"
            });
        }


        private void DrawWave(Wave w)
        {
            //chart_stockData.Annotations.Clear();
            DetectWaves();

            int s = w.StartIndex, e = w.EndIndex;
            if (s < 0 || e >= BindingCandleSticks.Count || s >= e) return;

            decimal lo = decimal.MaxValue,
                    hi = decimal.MinValue;

            for (int i = s; i <= e; i++)
            {
                if (BindingCandleSticks[i].Low < lo) lo = BindingCandleSticks[i].Low;
                if (BindingCandleSticks[i].High > hi) hi = BindingCandleSticks[i].High;
            }

            // Confirmations
            int confirms = CalculateConfirmations(s, e, hi, lo, true);
            label1.Text = $"Confirmations: {confirms}";
        }

        private void AddFibLines(decimal hi, decimal lo, int startIdx, int endIdx)
        {
            var area = chart_stockData.ChartAreas["area_OHLC"];
            double xl = startIdx + 0.5;
            double w = endIdx - startIdx;
            bool isDownwardWave = BindingCandleSticks[startIdx].Low > BindingCandleSticks[endIdx].Low;

            var fib = isDownwardWave
                ? new List<(decimal, string)>
                {
                    (hi, "0%"),
                    (hi - 0.236m*(hi-lo), "23.6%"),
                    (hi - 0.382m*(hi-lo), "38.2%"),
                    (hi - 0.5m*(hi-lo), "50%"),
                    (hi - 0.618m*(hi-lo), "61.8%"),
                    (hi - 0.764m*(hi-lo), "76.4%"),
                    (lo, "100%")
                }
                : new List<(decimal, string)>
                {
                    (lo, "0%"),
                    (lo + 0.236m*(hi-lo), "23.6%"),
                    (lo + 0.382m*(hi-lo), "38.2%"),
                    (lo + 0.5m*(hi-lo), "50%"),
                    (lo + 0.618m*(hi-lo), "61.8%"),
                    (lo + 0.764m*(hi-lo), "76.4%"),
                    (hi, "100%")
                };

            foreach (var (lvl, label) in fib)
            {
                chart_stockData.Annotations.Add(new HorizontalLineAnnotation
                {
                    AxisX = area.AxisX,
                    AxisY = area.AxisY,
                    ClipToChartArea = area.Name,
                    X = xl,
                    Width = w,
                    Y = (double)lvl,
                    LineColor = Color.Purple,
                    LineDashStyle = ChartDashStyle.Dash,
                    LineWidth = 1,
                    IsSizeAlwaysRelative = false
                });

                chart_stockData.Annotations.Add(new TextAnnotation
                {
                    AxisX = area.AxisX,
                    AxisY = area.AxisY,
                    X = xl + w + 0.5,
                    Y = (double)lvl,
                    Text = label,
                    Font = new Font("Arial", 8),
                    ForeColor = Color.Black
                });
            }
        }

        private void button_updateStockData_Click(object s, EventArgs e)
        {
            BindingCandleSticks.Clear();
            GetCandlesticksInDateRange(dateTimePickerStartDate.Value, dateTimePickerEndDate.Value);
            if (!BindingCandleSticks.Any())
            {
                MessageBox.Show($"No data in that range for {Path.GetFileNameWithoutExtension(FileName)}.");
                return;
            }
            UpdateChart();
            DetectWaves();
            UpdatePeaksAndValleys();
            rbRect = Rectangle.Empty;
        }

        private void AddEndpointBox(int idx, Color color)
        {
            var c = BindingCandleSticks[idx];
            chart_stockData.Annotations.Add(new RectangleAnnotation
            {
                AxisX = chart_stockData.ChartAreas["area_OHLC"].AxisX,
                AxisY = chart_stockData.ChartAreas["area_OHLC"].AxisY,
                ClipToChartArea = "area_OHLC",
                LineColor = color,
                LineWidth = 2,
                BackColor = Color.Transparent,
                X = idx + 0.25,
                Width = 0.5,
                Y = (double)c.High,
                Height = (double)(c.High - c.Low)
            });
        }

        private Rectangle HandleTL() => new Rectangle(
            rbRect.Left - RB_HANDLE / 2,
            rbRect.Top - RB_HANDLE / 2,
            RB_HANDLE, RB_HANDLE);

        private Rectangle HandleTR() => new Rectangle(
            rbRect.Right - RB_HANDLE / 2,
            rbRect.Top - RB_HANDLE / 2,
            RB_HANDLE, RB_HANDLE);

        private Rectangle HandleBL() => new Rectangle(
            rbRect.Left - RB_HANDLE / 2,
            rbRect.Bottom - RB_HANDLE / 2,
            RB_HANDLE, RB_HANDLE);

        private Rectangle HandleBR() => new Rectangle(
            rbRect.Right - RB_HANDLE / 2,
            rbRect.Bottom - RB_HANDLE / 2,
            RB_HANDLE, RB_HANDLE);

        private int PixelToIndex(int pixelX)
        {
            var area = chart_stockData.ChartAreas["area_OHLC"];
            double val = area.AxisX.PixelPositionToValue(pixelX);
            return (int)Math.Round(val);
        }

        private void ComputeSelectedIndices()
        {
            selStartIdx = selEndIdx = -1;
            if (rbRect == Rectangle.Empty) return;

            int a = PixelToIndex(rbRect.Left);
            int b = PixelToIndex(rbRect.Right);
            if (a > b) (a, b) = (b, a);

            a = Math.Max(0, Math.Min(a, BindingCandleSticks.Count - 1));
            b = Math.Max(0, Math.Min(b, BindingCandleSticks.Count - 1));

            if (b - a < 2) return;
            selStartIdx = a;
            selEndIdx = b;
        }

        private static bool InHandle(Point p, Rectangle h) => h.Contains(p);

        private List<(float yPix, string label)> CalculateFibonacciLines(Rectangle rectangle,
                                                                         ChartArea chartArea)
        {
            // actual prices at the two mouse points
            double priceStart = chartArea.AxisY.PixelPositionToValue(rbStartPx.Y);
            double priceEnd = chartArea.AxisY.PixelPositionToValue(rbEndPx.Y);

            bool isDownwardWave = priceStart > priceEnd;   // falling from left→right?

            double high = Math.Max(priceStart, priceEnd);
            double low = Math.Min(priceStart, priceEnd);
            double range = high - low;

            var levels = isDownwardWave
                ? new List<(double, string)>
                  {
                      (high,                     "0%"),
                      (high - 0.236 * range, "23.6%"),
                      (high - 0.382 * range, "38.2%"),
                      (high - 0.5   * range,  "50%"),
                      (high - 0.618 * range, "61.8%"),
                      (high - 0.764 * range, "76.4%"),
                      (low,                      "100%")
                  }
                : new List<(double, string)>
                  {
                      (low,                      "0%"),
                      (low  + 0.236 * range, "23.6%"),
                      (low  + 0.382 * range, "38.2%"),
                      (low  + 0.5   * range,  "50%"),
                      (low  + 0.618 * range, "61.8%"),
                      (low  + 0.764 * range, "76.4%"),
                      (high,                     "100%")
                  };

            // convert to pixels
            return levels.Select(lvl => (
                       (float)chartArea.AxisY.ValueToPixelPosition(lvl.Item1),
                       lvl.Item2)).ToList();
        }

        private void Chart_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            // Clear only selection-related annotations
            var toRemove = chart_stockData.Annotations
                .Where(a => a.Tag?.ToString() == "YellowDot" ||
                            a.Tag?.ToString() == "ConfirmationArrow" ||
                            a is LineAnnotation)
                .ToList();

            foreach (var annotation in toRemove)
            {
                chart_stockData.Annotations.Remove(annotation);
            }

            label1.Text = "Confirmations: 0";
            rbIsDragging = true;
            rbStartPx = rbEndPx = e.Location;
            rbRect = Rectangle.Empty;
            isOutOfBounds = false;
            chart_stockData.Invalidate();
        }

        private void Chart_MouseMove(object sender, MouseEventArgs e)
        {
            if (!rbIsDragging) return;

            rbEndPx = e.Location;
            rbRect = new Rectangle(
                Math.Min(rbStartPx.X, rbEndPx.X),
                Math.Min(rbStartPx.Y, rbEndPx.Y),
                Math.Abs(rbEndPx.X - rbStartPx.X),
                Math.Abs(rbEndPx.Y - rbStartPx.Y));

            ComputeSelectedIndices();
            int confirms = 0;
            if (selStartIdx >= 0 && selEndIdx >= 0)
            {
                var area = chart_stockData.ChartAreas["area_OHLC"];
                double priceStart = area.AxisY.PixelPositionToValue(rbStartPx.Y);
                double priceEnd = area.AxisY.PixelPositionToValue(rbEndPx.Y);
                decimal waveHi = (decimal)Math.Max(priceStart, priceEnd);
                decimal waveLo = (decimal)Math.Min(priceStart, priceEnd);

                confirms = CalculateConfirmations(selStartIdx, selEndIdx, waveHi, waveLo, false);
            }
            label1.Text = $"Confirmations: {confirms}";
            chart_stockData.Invalidate();
        }

        private void Chart_MouseUp(object sender, MouseEventArgs e)
        {
            if (!rbIsDragging) return;

            rbIsDragging = false;
            rbEndPx = e.Location;
            rbRect = new Rectangle(
                Math.Min(rbStartPx.X, rbEndPx.X),
                Math.Min(rbStartPx.Y, rbEndPx.Y),
                Math.Abs(rbEndPx.X - rbStartPx.X),
                Math.Abs(rbEndPx.Y - rbStartPx.Y));

            // Validate starting point AFTER selection is made
            int margin = hScrollBar_margin.Value;
            int startIdx = PixelToIndex(rbStartPx.X);

            if (BindingCandleSticks.Count <= 2 * margin ||
                startIdx < margin ||
                startIdx >= BindingCandleSticks.Count - margin ||
                (!IsPeak(startIdx, margin) && !IsValley(startIdx, margin)))
            {
                MessageBox.Show("Selection must start at a peak or valley. Please try again.");
                rbRect = Rectangle.Empty;
                chart_stockData.Invalidate();
                return;
            }

            ComputeSelectedIndices();
            if (selStartIdx >= 0 && selEndIdx >= 0)
            {
                var area = chart_stockData.ChartAreas["area_OHLC"];
                double priceStart = area.AxisY.PixelPositionToValue(rbStartPx.Y);
                double priceEnd = area.AxisY.PixelPositionToValue(rbEndPx.Y);
                decimal waveHi = (decimal)Math.Max(priceStart, priceEnd);
                decimal waveLo = (decimal)Math.Min(priceStart, priceEnd);

                int confirms = CalculateConfirmations(selStartIdx, selEndIdx, waveHi, waveLo, true);
                label1.Text = $"Confirmations: {confirms}";
            }

            chart_stockData.Invalidate();
        }

        private int CalculateConfirmations(int startIdx, int endIdx, decimal waveHi, decimal waveLo, bool addAnnotations = false)
        {
            var fib = new List<decimal>
    {
        waveHi,
        waveHi - 0.236m * (waveHi - waveLo),
        waveHi - 0.382m * (waveHi - waveLo),
        waveHi - 0.5m * (waveHi - waveLo),
        waveHi - 0.618m * (waveHi - waveLo),
        waveHi - 0.764m * (waveHi - waveLo),
        waveLo
    };

            var area = chart_stockData.ChartAreas["area_OHLC"];
            double rectLeftPx = Math.Min(rbStartPx.X, rbEndPx.X);
            double rectRightPx = Math.Max(rbStartPx.X, rbEndPx.X);
            double rectTopPx = Math.Min(rbStartPx.Y, rbEndPx.Y);
            double rectBottomPx = Math.Max(rbStartPx.Y, rbEndPx.Y);

            int confirms = 0;
            var addedLevels = new HashSet<decimal>(); // Track added levels to prevent duplicates

            for (int i = startIdx; i <= endIdx; i++)
            {
                double barXPx = area.AxisX.ValueToPixelPosition(i + 0.5);
                decimal H = BindingCandleSticks[i].High;
                decimal L = BindingCandleSticks[i].Low;
                double barHighPx = area.AxisY.ValueToPixelPosition((double)H);
                double barLowPx = area.AxisY.ValueToPixelPosition((double)L);

                if (barXPx < rectLeftPx || barXPx > rectRightPx ||
                    barHighPx < rectTopPx || barLowPx > rectBottomPx)
                    continue;

                foreach (var lvl in fib)
                {
                    // Calculate 1.5% tolerance for this Fibonacci level
                    decimal tolerance = lvl * 0.015m; // 1.5% of the level
                    bool hitHigh = Math.Abs(H - lvl) < tolerance;
                    bool hitLow = Math.Abs(L - lvl) < tolerance;

                    if (hitHigh || hitLow)
                    {
                        // Check if this level was already added
                        if (!addedLevels.Contains(lvl))
                        {
                            if (addAnnotations)
                            {
                                if (hitHigh)
                                    AddConfirmationArrow(i, (double)lvl, false); // Place at Fibonacci level
                                else
                                    AddConfirmationArrow(i, (double)lvl, true);
                            }
                            addedLevels.Add(lvl);
                            confirms++;
                        }
                        break; // Each candlestick contributes to one level max
                    }
                }
            }
            return confirms;
        }


        private void Chart_Paint(object sender, PaintEventArgs e)
        {
            if (rbRect == Rectangle.Empty) return;

            Rectangle currentRect = isSimulating ? fixedRbRect : rbRect;

            using (var pen = new Pen(isOutOfBounds ? Color.Red : Color.DodgerBlue, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
            {
                e.Graphics.DrawRectangle(pen, currentRect);
                // Draw diagonal line
                e.Graphics.DrawLine(pen, rbStartPx, rbEndPx);
            }

            // Draw simulation price line if applicable
            if (isSimulating || (btnIncrease.Enabled && currentSimulationPrice != 0))
            {
                var area = chart_stockData.ChartAreas["area_OHLC"];
                int priceY = (int)area.AxisY.ValueToPixelPosition(currentSimulationPrice);

                using (var linePen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawLine(linePen, currentRect.Left, priceY, currentRect.Right, priceY);
                }

                // Draw price label
                string priceText = currentSimulationPrice.ToString("F2");
                using (var textBrush = new SolidBrush(Color.Black))
                using (var font = new Font("Arial", 10, FontStyle.Bold))
                {
                    var textSize = e.Graphics.MeasureString(priceText, font);
                    var textRect = new Rectangle(currentRect.Right + 5, priceY - (int)textSize.Height / 2, (int)textSize.Width + 5, (int)textSize.Height);
                    e.Graphics.FillRectangle(Brushes.White, textRect);
                    e.Graphics.DrawString(priceText, font, textBrush, currentRect.Right + 5, priceY - textSize.Height / 2);
                }
            }

            if (!isOutOfBounds)
            {
                var chartArea = chart_stockData.ChartAreas["area_OHLC"];
                var fibLines = CalculateFibonacciLines(currentRect, chartArea);

                using (var fibPen = new Pen(Color.Purple, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
                using (var labelBrush = new SolidBrush(Color.Black))
                {
                    foreach (var (yPixel, label) in fibLines)
                    {
                        e.Graphics.DrawLine(fibPen, currentRect.Left, yPixel, currentRect.Right, yPixel);
                        e.Graphics.DrawString(label, new Font("Arial", 8), labelBrush, currentRect.Right + 5, yPixel - 6);
                    }
                }
            }

            using (var br = new SolidBrush(Color.DodgerBlue))
            {
                e.Graphics.FillEllipse(br, HandleTL());
                e.Graphics.FillEllipse(br, HandleTR());
                e.Graphics.FillEllipse(br, HandleBL());
                e.Graphics.FillEllipse(br, HandleBR());
            }
        }

        private void ScrollBarMargin_Scroll(object s, ScrollEventArgs e)
        {
            label_margin.Text = $"Margin: {hScrollBar_margin.Value}";
            DetectWaves();
            UpdatePeaksAndValleys();
            rbRect = Rectangle.Empty;
        }

        private void chart_stockData_Click(object s, EventArgs e) { }
        private void DateTimePickerStartDate_ValueChanged(object s, EventArgs e) { }
        private void LabelUpWaves_Click(object s, EventArgs e) { }
        private void LabelUpdateEndDate_Click(object s, EventArgs e) { }
        private void ChartDisplayForm_Load(object s, EventArgs e) { }
        private void label1_Click(object s, EventArgs e) { }

        private void label_updateStartDate_Click(object sender, EventArgs e)
        {

        }

        private void label_downWaves_Click(object sender, EventArgs e)
        {

        }

        private void label_upWaves_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void label_margin_Click(object sender, EventArgs e)
        {

        }

        private void label_updateEndDate_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePickerEndDate_ValueChanged(object sender, EventArgs e)
        {

        }
    }

    internal class Wave
    {
        public int StartIndex { get; }
        public int EndIndex { get; }
        public bool IsUp { get; }
        public string Label { get; }

        public Wave(int start, int end, string label, bool up)
        {
            StartIndex = start;
            EndIndex = end;
            Label = label;
            IsUp = up;
        }
        public override string ToString() => Label;
    }
}