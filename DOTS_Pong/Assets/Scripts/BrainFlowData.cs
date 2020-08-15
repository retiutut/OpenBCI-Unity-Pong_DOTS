using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Accord;
using Accord.Math;
using brainflow;
using System.Runtime.CompilerServices;
using UnityEditor;

public class BrainFlowData : MonoBehaviour
{
    private BoardShim board_shim = null;
    private int board_id = (int)BoardIds.SYNTHETIC_BOARD;
    private int samplingRate = 0;
    private int[] eegChannels = null;
    private int[] accelChannels = null;
    private bool isSynthetic = false;
    private bool isGanglion = true;

    public static int numChan = 2;
    public static double[] ratios = new double[numChan];
    private double[] minimums = new double[numChan];
    private double[] maximums = new double[numChan];
    public static double[] accelData = null;

    // Start is called before the first frame update
    void Start()
    {
        
        Debug.Log("MADE NEW BRAINFLOW OBJECT");
        try
        {
            BoardShim.set_log_file("brainflow_log.txt");
            BoardShim.enable_dev_board_logger();

            BrainFlowInputParams input_params = new BrainFlowInputParams();
           
            if (!isSynthetic)
            {
                input_params.serial_port = "COM4";
                board_id = 0;
            }

            if (isGanglion)
            {
                input_params.serial_port = "COM3";
                board_id = 1;
            }

            board_shim = new BoardShim(board_id, input_params);
            board_shim.prepare_session();
            //board_shim.config_board("n");
            board_shim.start_stream(450000, "file://brainflow_data.csv:w");

            samplingRate = BoardShim.get_sampling_rate(board_id);
            eegChannels = BoardShim.get_eeg_channels(board_id);
            accelChannels = BoardShim.get_accel_channels(board_id);
            Debug.Log("Brainflow streaming was started");
        }
        catch (BrainFlowException e)
        {
            Debug.Log(e);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (board_shim == null)
        {
            return;
        }

        int number_of_data_points = DataFilter.get_nearest_power_of_two(samplingRate);
        double[,] data = board_shim.get_current_board_data(number_of_data_points);

        if (data.GetRow(0).Length < number_of_data_points)
        {
            // wait for more data
            return;
        }

        //accelData = data.GetRow(accelChannels[0]);
        //Debug.Log(String.Format("[{0}]", string.Join(", ", accelData)));

        double[] filtered;
        for (int i = 0; i < numChan; i++)
        {
            //Debug.Log("Before processing:");
            //Debug.Log(String.Format("[{0}]", string.Join(", ", data.GetRow(eegChannels[i]))));

            filtered = DataFilter.perform_bandpass(data.GetRow(eegChannels[i]), BoardShim.get_sampling_rate(board_id), 15.0, 5.0, 2, (int)FilterTypes.BUTTERWORTH, 0.0);
            //Debug.Log("Filtered channel " + eegChannels[i]);
            //Debug.Log(String.Format("[{0}]", string.Join(", ", filtered)));

            /*
            // calc bandpowers per channel
            for (int i = 0; i < numChan; i++)
            {
                Tuple<double[], double[]> psd = DataFilter.get_psd(data.GetRow(eegChannels[i]), 0,
                    data.GetRow(eegChannels[i]).Length, samplingRate, (int)WindowFunctions.HANNING);
                double band_power_alpha = DataFilter.get_band_power(psd, 7.0, 13.0);
                double band_power_beta = DataFilter.get_band_power(psd, 14.0, 30.0);
                double band_power_gamma = DataFilter.get_band_power(psd, 31.0, 100.0);
                //double ratio = (band_power_alpha / band_power_beta);

                //Debug.Log("Channel " + i + " | Alpha/Beta Ratio:" + ratio);
                ratios[i] = normalized_value(i, band_power_alpha);
            }
            */

            /////WORKING HERE
            //for data size for 1 channel, add absolute value of all values, and divide by size
            double myAverage = 0f;
            float smoothing = .25f;
            float averagePeriod = BoardShim.get_sampling_rate(board_id) * smoothing;
            float acceptableLimitUV = 200f;
            for (int j = filtered.Length - (int) averagePeriod; j < filtered.Length; j++)
            {
                double value = Math.Abs(filtered[j]);
                if (value <= acceptableLimitUV)
                { //prevent BIG spikes from effecting the average
                    myAverage += value;  //add value to average ... we will soon divide by # of packets
                }
                else
                {
                    myAverage += acceptableLimitUV; //if it's greater than the limit, just add the limit
                }
            }
            myAverage = myAverage / averagePeriod; // float(cfc.averagePeriod); //finishing the average
            

            double output_normalized = normalized_value(i, myAverage);

            if (output_normalized < 0)
            {
                output_normalized = 0; //always make sure this value is >= 0
            }
            //output_adjusted = ((-0.1d / (output_normalized * 255d)) + 255d);
            ratios[i] = output_normalized;

            Debug.Log("CHAN " + i + " | output_norm = " + output_normalized);
        }


    }

    private void OnDestroy()
    {
        if (board_shim != null)
        {
            try
            {
                board_shim.release_session();
            }
            catch (BrainFlowException e)
            {
                Debug.Log(e);
            }
            Debug.Log("Brainflow streaming was stopped");
        }
    }

    /**
     * Calculates a value between 0 and 1, given the precondition that value
     * is between min and max. 0 means value = max, and 1 means value = min.
     */
    double normalized_value(int channel, double value)
    {
        if (value > maximums[channel])
        {
            maximums[channel] = value;
        }

        if (value < minimums[channel])
        {
            minimums[channel] = value;
        }

        return ((value - minimums[channel]) / (maximums[channel] - minimums[channel]));
    }

}