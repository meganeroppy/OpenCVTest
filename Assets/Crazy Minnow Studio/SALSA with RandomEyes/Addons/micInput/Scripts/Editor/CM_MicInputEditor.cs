using UnityEngine;
using UnityEditor;

namespace CrazyMinnow.SALSA
{
	/// <summary>
	/// Custom Inspector for CM_MicInput.cs v1.6.0
	/// </summary>
	[CustomEditor(typeof(CM_MicInput))]
	public class CM_MicInputEditor : Editor {

		string[] sampleRates = { "default", "9600", "11025", "22050", "44100" };
		string[] micDevices;
		int rateIndex = 0;
		int deviceIndex = 0;

		CM_MicInput micInput;

		void OnEnable()
		{
			micInput = (CM_MicInput)target;
			GetMicList();
			deviceIndex = GetMicListIndex(micInput.selectedMic);  // obtain the microphone device setting from micInput instance
			rateIndex = GetRateIndex(micInput.sampleRate);
		}

		public override void OnInspectorGUI()
		{
			micInput.audioSrc = (AudioSource) EditorGUILayout.ObjectField("Audio Source", micInput.audioSrc, typeof(AudioSource), true);
			micInput.isDebug = EditorGUILayout.Toggle("Debug Mode", micInput.isDebug);
			micInput.isAutoStart = EditorGUILayout.Toggle("AutoStart Mode", micInput.isAutoStart);
			micInput.isMuted = EditorGUILayout.Toggle("Mute Microphone", micInput.isMuted);

			EditorGUI.BeginChangeCheck();
			int conversionCatch;
			rateIndex = EditorGUILayout.Popup("Sample Rate", rateIndex, sampleRates);
			if ( int.TryParse(sampleRates[rateIndex], out conversionCatch) )
				micInput.sampleRate = conversionCatch;
			else
				micInput.sampleRate = 0;
			if ( EditorGUI.EndChangeCheck() )
			{
				micInput.StopMicrophone(micInput.selectedMic);
				micInput.StartMicrophone(micInput.selectedMic);
			}

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if ( GUILayout.Button("Refresh Mic List", GUILayout.ExpandWidth(false)) )
			{
				GetMicList();
				deviceIndex = GetMicListIndex(micInput.selectedMic); // obtain the microphone device setting from micInput instance (IF POSSIBLE)
			}
			GUILayout.EndHorizontal();


			// get selection index, then parse micDevices[] for the appropriate device name
			EditorGUI.BeginChangeCheck();

			string prevMic = micInput.selectedMic;

			deviceIndex = EditorGUILayout.Popup("Available Microphones", deviceIndex, micDevices);
			if ( deviceIndex > 0 )
				micInput.selectedMic = micDevices[deviceIndex];
			else
				micInput.selectedMic = default(string);
			if ( EditorGUI.EndChangeCheck() )
			{
				micInput.StopMicrophone(prevMic);
				micInput.StartMicrophone(micInput.selectedMic);
			}
		}

		void GetMicList()
		{
			if ( Microphone.devices.Length > 0 )
			{
				micDevices = new string[Microphone.devices.Length + 1];

				// create an entry for 'Default' mic
				micDevices[0] = "Default";

				for ( int i = 0; i < Microphone.devices.Length; i++ )
				{
					micDevices[i + 1] = Microphone.devices[i];
				}
			}
			else
			{
				micDevices = new string[1] { "ERROR - no microphones available" };
			}
		}

		int GetMicListIndex(string selected)
		{
			for ( int i = 0; i < micDevices.Length; i++ )
			{
				if ( micDevices[i] == selected )
				{
					return i;
				}
			}

			return 0;	// return default
		}

		int GetRateIndex(int rate)
		{
			for ( int i = 0; i < sampleRates.Length; i++ )
			{
				if ( rate.ToString() == sampleRates[i] )
				{
					return i;
				}
			}

			return 0;	// return default
		}
	}
}
