using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[AlwaysSynchronizeSystem]
public class PlayerInputSystem : JobComponentSystem
{
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		Entities
		.WithoutBurst()
		.ForEach((ref PaddleMovementData moveData, in PaddleInputData inputData) =>
		{
			moveData.direction = 0;

			moveData.direction += Input.GetKey(inputData.upKey) ? 1 : 0;
			moveData.direction -= Input.GetKey(inputData.downKey) ? 1 : 0;

            double data = (BrainFlowData.ratios[6] + BrainFlowData.ratios[7]) /  2;
			Debug.Log(data);
			GameManager.main.updatePlayerData(string.Format("{0:N2}", data));
			moveData.direction += data > .7d ? 1 : -1;
		}).Run();

		return default;
	}
}
