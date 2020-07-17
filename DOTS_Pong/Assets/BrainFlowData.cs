using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Accord;
using Accord.Math;
using brainflow;

public class BrainFlowData : MonoBehaviour
{
    private BoardShim board_shim = null;
    private int samplingRate = 0;
    private int[] eegChannels = null;

    public int numChan = 2;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            BoardShim.set_log_file("brainflow_log.txt");
            BoardShim.enable_dev_board_logger();

            BrainFlowInputParams input_params = new BrainFlowInputParams();
            int board_id = (int)BoardIds.SYNTHETIC_BOARD;
            input_params.serial_port = "COM4";
            board_id = 0;
            board_shim = new BoardShim(board_id, input_params);
            board_shim.prepare_session();
            board_shim.start_stream(450000, "file://brainflow_data.csv:w");

            samplingRate = BoardShim.get_sampling_rate(board_id);
            eegChannels = BoardShim.get_eeg_channels(board_id);
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
        // calc bandpowers per channel
        for (int i = 0; i < numChan; i++)
        {
            Tuple<double[], double[]> psd = DataFilter.get_psd(data.GetRow(eegChannels[i]), 0,
                data.GetRow(eegChannels[i]).Length, samplingRate, (int)WindowFunctions.HANNING);
            double band_power_alpha = DataFilter.get_band_power(psd, 7.0, 13.0);
            double band_power_beta = DataFilter.get_band_power(psd, 14.0, 30.0);
            double ratio = (band_power_alpha / band_power_beta);
            Debug.Log("Channel " + i + " | Alpha/Beta Ratio:" + ratio);
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

}