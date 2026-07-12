using UnityEngine;
using UnityEngine.UI;

namespace UnboundArcana.Sandbox.UI
{
	public class PlaytestWavePanel : MonoBehaviour
	{
		public Text text;

		private int wave;
		private string status;

		public void SetWave(int value)
		{
			wave = value;
			Refresh();
		}

		public void SetStatus(string value)
		{
			status = value;
			Refresh();
		}

		private void Refresh()
		{
			text.text =
				"Wave: " + wave +
				"\n\n" +
				status;
		}
	}
}