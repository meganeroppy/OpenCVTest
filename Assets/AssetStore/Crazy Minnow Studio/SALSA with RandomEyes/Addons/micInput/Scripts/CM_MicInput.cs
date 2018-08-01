using UnityEngine;
using System.Collections;

namespace CrazyMinnow.SALSA
{
	[AddComponentMenu("Crazy Minnow Studio/Addons/micInput")]
	public class CM_MicInput : MonoBehaviour
	{

		// RELEASE NOTES: 
		//			1.6.0 ~ StartMicrophone(): moved the blocking while loop to a
		//						non-blocking coroutine.
		//				  ~ cleaned up and optimized logic and logging.
		//				  ~ default sampleRate changed to 22050.
		//				  ~ CheckFreqCapability() functionality altered to set 
		//						sampleRate rather than simply confirm it.
		//				  + new isRecording read-only property to see if mic is
		//						started and recording.
		//				  + new isAutoStart mode.
		//				  + added additional checks and logging for missing
		//						AudioSource or mic references. Breaking actions now
		//						throw LogWarning() in isDebug.
		//				  + added logging during coroutines if isDebug.
		//				  + added additional check for sampleRate prior to starting
		//						the microphone. Ensures valid freq with Editor script
		//						processing during runtime.
		//			1.5.0 - removed dependency on Salsa2D or Salsa3D type (requires 
		//						SALSA 1.3.3+). SALSA components are no longer auto-
		//						added to the gameObject when micInput is attached.
		//				  - removed dependency on local attached AudioSource: now 
		//						public and if not found, will look for local 
		//						AudioSource component or throw log error. An
		//						AudioSource component is no longer automatically 
		//						added to the gameObject when micInput is attached.
		//				  ~ micInput now looks for an AudioSource in a coroutine to
		//						ensure runtime-created AudioSources have sufficient
		//						time to spin up. This is beneficial for workflows
		//						such as: SALSA with UMA2.
		//				  ~ It is no longer necessary to attach the micInput
		//						addon to the same GameObject SALSA is attached to.
		//			1.0.8 + microphone selection & basic custom inspector,
		//						it is now possible to select a particular microphone,
		//						allowing for multiple microphones input to separate
		//						SALSA-enabled characters. Now requires passing a string
		//						(can be empty string == default mic) to the micInput 
		//						api methods.
		//				  ~ isMuted defaulted to false to allow for Unity 5.2+
		//						functionality.
		//			1.0.7 + additional error trapping for no mic attached.
		//			1.0.6 ~ StartMicrophone() & StopMicrophone() are now public
		//			1.0.5 + Support for Unity 5: depricated 'audio' keyword replaced
		//						with AudioSource component reference.
		//			1.0.4 + Add component menu item --
		//						(Crazy Minnow Studio/Addons/MicInput)
		//			1.0.3 + CrazyMinnow.SALSA namespace
		//				  + filename/classname change
		//			1.0.2 ~ modified OnApplicationUpdate/Pause to more correctly 
		//						detect on desktop and mobile.
		//			1.0.1 - removed runInBackground option
		//			1.0.0 : initial release
		// ==========================================================================
		// INSTRUCTIONS: (visit crazyminnowstudio.com for latest instructions/info)
		//
		// PURPOSE: This script provides simple real-time Microphone input to the
		//		Salsa component. It links up the *default* microphone as an 
		//		AudioSource for SALSA. You *must* have at least one microphone 
		//		attached, enabled, and working.
		// ==========================================================================
		// DISCLAIMER: While every attempt has been made to ensure the safe content 
		//		and operation of these files, they are provided as-is, without 
		//		warranty or guarantee of any kind. By downloading and using these 
		//		files you are accepting any and all risks associated and release 
		//		Crazy Minnow Studio, LLC of any and all liability.
		// ==========================================================================

		public int sampleRate = 0;
		public bool isAutoStart = true;
		public bool isDebug = false;
		public bool isMuted = false;
		public string selectedMic = default(string);
		public AudioSource audioSrc;
		public bool isRecording
		{
			get
			{
				return isWiredUp;
			}
		}

		private bool isWiredUp = false;
		private bool isMicAvailable = false;
		private float coroutineLoggingIfDebug = 5f; // every 5 seconds

		void Start()
		{
			// Wiring up is now done in a coroutine to wait for the availability of an AudioSource. This
			//		modification is beneficial when integrating with runtime-created AudioSources such as 
			//		the case with the SALSA UMA2 workflow. 
			StartCoroutine(Wireup());

		} // end Start()



		// Check Microphone.devices list. If at least one device exists,
		//		there will be a default device and we'll use it.
		bool CheckForAvailableMicrophone(string mic)
		{
			// if mic is not specified, check for availability of any mic (Microphone.devices.Length > 0)
			if ( string.IsNullOrEmpty(mic) )
			{
				if ( isDebug )
					Debug.Log("[CheckForAvailableMicrophone()]: INFO: Microphone.device not specified.");

				if ( Microphone.devices.Length > 0 )
				{
					if ( isDebug )
						Debug.Log("[CheckForAvailableMicrophone()]: INFO: A default Microphone.device is available.");

					return true;

				}
				else
				{
					if ( isDebug )
						Debug.LogWarning("[CheckForAvailableMicrophone()] WARNING: no microphone devices listed.");

					return false;
				}
			}
			else
			{
				for ( int i = 0; i < Microphone.devices.Length; i++ )
				{
					if ( Microphone.devices[i] == mic )
					{
						if ( isDebug )
							Debug.Log("[CheckForAvailableMicrophone()] INFO: Microphone: " + mic + " IS available.");

						return true;
					}
				}

				if ( isDebug )
					Debug.LogWarning("[CheckForAvailableMicrophone()] WARNING: Microphone: " + mic + " is NOT available.");

				return false;
			}

		} // end CheckForAvailableMicrophone()



		// Check the available recording frequencies reported by the Microphone.
		//	- NOTE: a return of 0 indicates no limit.
		//	- If no limit, we'll use 44100, otherwise, we'll use the highest
		//		available frequency.
		bool CheckFreqCapability(string mic)
		{
			int bestRate = 0;

			int minFreq = -1;
			int maxFreq = -1;
			// Check for usable recording frequencies (0 = no limit).
			Microphone.GetDeviceCaps(mic, out minFreq, out maxFreq);

			if ( ( minFreq == 0 ) && ( maxFreq == 0 ) )
			{
				bestRate = 22050;

				if ( isDebug )
					Debug.Log("[CheckFreqCapability()] INFO: " + ( string.IsNullOrEmpty(mic) ? "Microphone (default)" : mic ) + " supports all frequencies.");
			}
			else
			{
				bestRate = maxFreq;

				if ( isDebug )
					Debug.Log("[CheckFreqCapability()] INFO: " + ( string.IsNullOrEmpty(mic) ? "Microphone (default)" : mic ) + " supports min: " + minFreq + " max: " + maxFreq + ".");
			}

			if ( sampleRate == 0 || sampleRate > bestRate ) // sampleRate not set by user or set too high
			{
				sampleRate = bestRate;
			}
			else if ( sampleRate < minFreq ) // sampleRate set too low
			{
				sampleRate = minFreq;
			}
			// otherwise use sampleRate set by user

			if ( isDebug )
				Debug.Log("[CheckFreqCapability()] INFO: " + ( string.IsNullOrEmpty(mic) ? "Microphone (default)" : mic ) + " sampleRate set to: " + sampleRate);

			return true;

		} // end CheckFreqCapability()



		public void StartMicrophone(string mic)
		{
			// failsafe exit if no mic found.
			if ( !CheckForAvailableMicrophone(mic) )
			{
				if ( isDebug )
				{
					Debug.LogWarning("[StartMicrophone()] WARNING: no microphone detected.");
				}
				return;
			}

			if ( audioSrc && !isWiredUp )
			{
				// confirm sampleRate is valid prior to Microphone.Start()
				if ( !CheckFreqCapability(mic) )
				{
					Debug.LogWarning("[StartMicrophone()] WARNING: cannot start, invalid sampleRate: " + sampleRate.ToString());
					return;
				}

				audioSrc.clip = Microphone.Start(mic, true, 10, sampleRate);

				// Set the AudioSource to loop for continuous playback.
				audioSrc.loop = true;

				// If you want to hear the mic input, set this to false.
				audioSrc.mute = isMuted;

				// Wait for the microphone to start recording data
				StartCoroutine(WaitForMic(mic));
			}
			else
			{
				if ( isDebug )
					Debug.LogWarning("[StartMicrophone()] WARNING: backing out, AudioSource is NOT available.");
			}

		} // end StartMicrophone()



		public void StopMicrophone(string mic = null)
		{
			if ( audioSrc && isWiredUp )
			{
				// Stop the AudioSource.
				audioSrc.Stop();
				audioSrc.clip = null;

				// Stop the Microphone playback.
				Microphone.End(mic);

				// The AudioSource will be disconnected, flag it.
				isWiredUp = false;

				if ( isDebug )
					Debug.Log("[StopMicrophone()] INFO: " + ( string.IsNullOrEmpty(mic) ? "Microphone (default)" : mic ) + " is stopped.");
			}
			else
			{
				if ( isDebug )
					Debug.LogWarning("[StopMicrophone()] WARNING: backing out, AudioSource is NOT available.");
			}


		} // end StopMicrophone()



		void OnApplicationFocus(bool isFocused)
		{
			if ( isDebug )
			{
				Debug.Log("[OnApplicationFocus()] INFO: isFocused:" + isFocused + " isWired:" + isWiredUp + " runInBack:" + Application.runInBackground);
			}

			// If the application is focused and micInput is not wired up.
			if ( isFocused && !isWiredUp && !Application.runInBackground )
			{

				if ( isMicAvailable && audioSrc )
				{
					if ( isDebug )
						Debug.Log("[OnApplicationFocus()] INFO: StartMicrophone()");

					StartMicrophone(selectedMic);
				}
				else
				{
					if ( isDebug )
						Debug.LogWarning("[OnApplicationFocus()] WARNING: cannot StartMicrophone(); isMicAvailable: " + isMicAvailable + "- AudioSource " + ( audioSrc ? "IS available." : "is NOT available." ));
				}
			}

			// If the application loses focus, stop the Microphone and AudioSource.
			//	- This allows the AudioSource and micInput to be re-wired when the
			//		application is back in focus. Helps prevent it from getting 
			//		out of sync. If Application is set to runInBackground, the
			//		application will not respond to focus changes and will continue
			//		running.
			if ( !isFocused && isWiredUp && !Application.runInBackground )
			{
				if ( isMicAvailable && audioSrc )
				{
					if ( isDebug )
						Debug.Log("[OnApplicationFocus()] StopMicrophone()");

					StopMicrophone(selectedMic);
				}
				else
				{
					if ( isDebug )
						Debug.LogWarning("[OnApplicationFocus()] WARNING: cannot StopMicrophone(); isMicAvailable: " + isMicAvailable + "- AudioSource " + ( audioSrc ? "IS available." : "is NOT available." ));
				}
			}

		} // end OnApplicationFocus()



		void OnApplicationPause(bool isPaused)
		{
			if ( isDebug )
			{
				Debug.Log("[OnApplicationPause()] INFO: isPaused:" + isPaused + " isWired:" + isWiredUp + " runInBack:" + Application.runInBackground);
			}

			// If the application is focused and micInput is not wired up.
			if ( !isPaused && !isWiredUp && !Application.runInBackground )
			{
				if ( isMicAvailable && audioSrc )
				{
					if ( isDebug )
						Debug.Log("[OnApplicationPause()] INFO: StartMicrophone()");

					StartMicrophone(selectedMic);
				}
				else
				{
					if ( isDebug )
						Debug.LogWarning("[OnApplicationPause()] WARNING: cannot StartMicrophone(); isMicAvailable: " + isMicAvailable + "- AudioSource " + ( audioSrc ? "IS available." : "is NOT available." ));
				}
			}
			// If the application is paused, stop the Microphone and AudioSource.
			//	- This allows the AudioSource and micInput to be re-wired when the
			//		application is unpaused. Helps prevent it all from getting 
			//		out of sync. If Application is set to runInBackground, the
			//		application will not respond to pause changes and will continue
			//		running.
			if ( isPaused && isWiredUp && !Application.runInBackground )
			{
				if ( isMicAvailable && audioSrc )
				{
					if ( isDebug )
						Debug.Log("[OnApplicationPause()] StopMicrophone()");

					StopMicrophone(selectedMic);
				}
				else
				{
					if ( isDebug )
						Debug.LogWarning("[OnApplicationPause()] WARNING: cannot StopMicrophone(); isMicAvailable: " + isMicAvailable + "- AudioSource " + ( audioSrc ? "IS available." : "is NOT available." ));
				}
			}

		} // end OnApplicationPause()



		IEnumerator WaitForMic(string mic)
		{
			float timeCheck = Time.time;

			// Let the Microphone start filling the buffer prior to activating the AudioSource.
			while ( !( Microphone.GetPosition(mic) > 0 ) )
			{
				if ( isDebug && Time.time - timeCheck > coroutineLoggingIfDebug )
				{
					Debug.Log("[WaitForMic()] - is waiting for the mic to record.");
					timeCheck = Time.time;
				}

				// Wait for Microphone to start gathering data.
				yield return null;
			}

			// If the AudioSource was successfully assigned, play(activate) the AudioSource.
			if ( audioSrc.clip )
			{
				audioSrc.Play();
				isWiredUp = true;

				if ( isDebug )
					Debug.Log("[WaitForMic()] INFO: " + ( string.IsNullOrEmpty(mic) ? "Microphone (default)" : mic ) + " is started.");
			}
			else
			{
				Debug.LogError("[WaitForMic()] ERROR: AudioSource has no clip assigned.");
			}
		} // end WaitForMic()



		IEnumerator Wireup()
		{
			float timeCheck = Time.time;

			// confirm an AudioSource is available
			while ( audioSrc == null )
			{
				if ( isDebug && Time.time - timeCheck > coroutineLoggingIfDebug )
				{
					Debug.Log("[Wireup()] - is waiting for an AudioSource.");
					timeCheck = Time.time;
				}

				// no AudioSource found, look for an AudioSource attached to this GameObject
				audioSrc = GetComponent<AudioSource>();

				yield return null;
			}

			// Perform a check for an attached and enabled Microphone.device
			if ( CheckForAvailableMicrophone(selectedMic) )
			{
				isMicAvailable = true;

				if ( isAutoStart )
					StartMicrophone(selectedMic);
			}
			else
			{
				Debug.Log("[Wireup()] WARNING: no microphone detected.");
			}

		} // end Wireup()

	} // end CM_MicInput Class
}