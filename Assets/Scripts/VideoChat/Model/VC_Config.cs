using agora_gaming_rtc;
#if (UNITY_2018_3_OR_NEWER)
#endif

public class VC_Config
{
	public CameraCapturerConfiguration myCapturerConfig;
	public VideoEncoderConfiguration myEncoderConfig;
	public int AUDIO_PROFILE_TYPE_int;
	public int AUDIO_SCENARIO_TYPE_int;
	public string AppID;
	public float DistanceUnit;

	public VC_Config()
	{
		AppID = "a3089f2f7ff5488aac4f20d3d845c57c";
		// 捕获相关
		myCapturerConfig = new CameraCapturerConfiguration();
		myCapturerConfig.preference = CAPTURER_OUTPUT_PREFERENCE.CAPTURER_OUTPUT_PREFERENCE_MANUAL;
		myCapturerConfig.cameraDirection = CAMERA_DIRECTION.CAMERA_REAR;
		myCapturerConfig.captureWidth = 576;
		myCapturerConfig.captureHeight = 432;
		// 编码推流相关
		myEncoderConfig = new VideoEncoderConfiguration();
		myEncoderConfig.dimensions.height = 216;
		myEncoderConfig.dimensions.width = 288;
		myEncoderConfig.frameRate = FRAME_RATE.FRAME_RATE_FPS_15;
		myEncoderConfig.bitrate = 400;

		// 音频相关
		AUDIO_PROFILE_TYPE_int = (int)AUDIO_PROFILE_TYPE.AUDIO_PROFILE_DEFAULT;
		AUDIO_SCENARIO_TYPE_int = (int)AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT;

		// 场景距离基数
		DistanceUnit = 1.25f;
	}

}
