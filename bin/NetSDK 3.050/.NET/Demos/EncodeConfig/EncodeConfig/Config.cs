using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace EncodeConfig
{
    public class Config
    {
        public const string CFG_CMD_ENCODE = "Encode";

        // 图像通道属性信息
        public struct NET_CFG_ENCODE_INFO
        {
            public int nChannelID;							// 通道号(0开始),获取时，该字段有效；设置时，该字段无效
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string szChnName;		// 无效字段
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public NET_CFG_VIDEOENC_OPT[] stuMainStream;	// 主码流，0－普通录像，1-动检录像，2－报警录像
            public int nValidCountMainStream;              // 主码流数组中有效的个数
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public NET_CFG_VIDEOENC_OPT[] stuExtraStream;// 辅码流，0－辅码流1，1－辅码流2，2－辅码流3
            public int nValidCountExtraStream;             // 辅码流数组中有效的个数
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public NET_CFG_VIDEOENC_OPT[] stuSnapFormat;	// 抓图，0－普通抓图，1－动检抓图，2－报警抓图
            public int nValidCountSnapFormat;              // 抓图数组中有效的个数
            public uint dwCoverAbilityMask;					// 无效字段
            public uint dwCoverEnableMask;					// 无效字段
            public NET_CFG_VIDEO_COVER stuVideoCover;						// 无效字段
            public NET_CFG_OSD_INFO stuChnTitle;						// 无效字段
            public NET_CFG_OSD_INFO stuTimeTitle;						// 无效字段
            public NET_CFG_COLOR_INFO stuVideoColor;						// 无效字段
            public EM_CFG_AUDIO_FORMAT emAudioFormat;                      // 无效字段
            public int nProtocolVer;						// 协议版本号, 只读,获取时，该字段有效；设置时，该字段无效
        }

        // 视频编码参数
        public struct NET_CFG_VIDEOENC_OPT
        {
            // 能力
            public byte abVideoEnable;
            public byte abAudioEnable;
            public byte abSnapEnable;
            public byte abAudioAdd;                 // 音频叠加能力
            public byte abAudioFormat;

            // 信息
            public bool bVideoEnable;				// 视频使能
            public NET_CFG_VIDEO_FORMAT stuVideoFormat;				// 视频格式
            public bool bAudioEnable;				// 音频使能
            public bool bSnapEnable;				// 定时抓图使能
            public bool bAudioAddEnable;            // 音频叠加使能
            public NET_CFG_AUDIO_ENCODE_FORMAT stuAudioFormat;			// 音频格式
        }

        // 多区域遮挡配置
        public struct NET_CFG_VIDEO_COVER
        {
            public int nTotalBlocks;						// 支持的遮挡块数
            public int nCurBlocks;							// 已设置的块数
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public NET_CFG_COVER_INFO[] stuCoverBlock;	// 覆盖的区域	
        }

        // 遮挡信息
        public struct NET_CFG_COVER_INFO
        {
            // 能力
            public byte abBlockType;
            public byte abEncodeBlend;
            public byte abPreviewBlend;

            // 信息
            public NET_CFG_RECT stuRect;					// 覆盖的区域坐标
            public NET_CFG_RGBA stuColor;					// 覆盖的颜色
            public int nBlockType;					// 覆盖方式；0－黑块，1－马赛克
            public int nEncodeBlend;				// 编码级遮挡；1－生效，0－不生效
            public int nPreviewBlend;				// 预览遮挡；1－生效，0－不生效
        }

        // 区域信息
        public struct NET_CFG_RECT
        {
            public int nLeft;
            public int nTop;
            public int nRight;
            public int nBottom;
        }

        // RGBA信息
        public struct NET_CFG_RGBA
        {
            public int nRed;
            public int nGreen;
            public int nBlue;
            public int nAlpha;
        }

        // 视频格式
        public struct NET_CFG_VIDEO_FORMAT
        {
            // 能力
            public byte abCompression;
            public byte abWidth;
            public byte abHeight;
            public byte abBitRateControl;
            public byte abBitRate;
            public byte abFrameRate;
            public byte abIFrameInterval;
            public byte abImageQuality;
            public byte abFrameType;
            public byte abProfile;

            // 信息
            public EM_CFG_VIDEO_COMPRESSION emCompression;			// 视频压缩格式
            public int nWidth;						// 视频宽度
            public int nHeight;					// 视频高度
            public EM_CFG_BITRATE_CONTROL emBitRateControl;			// 码流控制模式
            public int nBitRate;					// 视频码流(kbps)
            public float nFrameRate;					// 视频帧率
            public int nIFrameInterval;			// I帧间隔(1-100)，比如50表示每49个B帧或P帧，设置一个I帧。
            public EM_CFG_IMAGE_QUALITY emImageQuality;				// 图像质量
            public int nFrameType;					// 打包模式，0－DHAV，1－"PS"
            public EM_CFG_H264_PROFILE_RANK emProfile;                // H.264编码级别
        }

        // 音频格式
        public struct NET_CFG_AUDIO_ENCODE_FORMAT
        {
            // 能力
            public byte abCompression;
            public byte abDepth;
            public byte abFrequency;
            public byte abMode;
            public byte abFrameType;
            public byte abPacketPeriod;

            // 信息
            public EM_CFG_AUDIO_FORMAT emCompression;				// 音频压缩模式
            public int nDepth;						// 音频采样深度
            public int nFrequency;					// 音频采样频率
            public int nMode;						// 音频编码模式
            public int nFrameType;					// 音频打包模式, 0-DHAV, 1-PS
            public int nPacketPeriod;				// 音频打包周期, ms
        }

        // 音频编码模式
        public enum EM_CFG_AUDIO_FORMAT
        {
            G711A,                              // G711a
            PCM,                                // PCM
            G711U,                              // G711u
            AMR,                                // AMR
            AAC,                                // AAC
        }

        // 视频压缩格式
        public enum EM_CFG_VIDEO_COMPRESSION
        {
            MPEG4,								// MPEG4
            MS_MPEG4,							// MS-MPEG4
            MPEG2,								// MPEG2
            MPEG1,								// MPEG1
            H263,								// H.263
            MJPG,								// MJPG
            FCC_MPEG4,							// FCC-MPEG4
            H264,								// H.264
            H265,								// H.265
            SVAC,								// SVAC
        }

        // 码流控制模式
        public enum EM_CFG_BITRATE_CONTROL
        {
            CBR,									// 固定码流
            VBR,									// 可变码流
        }


        // 画质
        public enum EM_CFG_IMAGE_QUALITY
        {
            Q10 = 1,							// 图像质量10%
            Q30,								// 图像质量30%
            Q50,								// 图像质量50%
            Q60,								// 图像质量60%
            Q80,								// 图像质量80%
            Q100,								// 图像质量100%
        }

        // H264 编码级别
        public enum EM_CFG_H264_PROFILE_RANK
        {
            BASELINE = 1,                       // 提供I/P帧，仅支持progressive(逐行扫描)和CAVLC
            MAIN,                               // 提供I/P/B帧，支持progressiv和interlaced，提供CAVLC或CABAC
            EXTENDED,                           // 提供I/P/B/SP/SI帧，仅支持progressive(逐行扫描)和CAVLC
            HIGH,                               // 即FRExt，Main_Profile基础上新增：8x8 intra prediction(8x8 帧内预测), custom 
            // quant(自定义量化), lossless video coding(无损视频编码), 更多的yuv格式
        }

        // OSD信息
        public struct NET_CFG_OSD_INFO
        {
            // 能力
            public byte abShowEnable;

            // 信息
            public NET_CFG_RGBA stuFrontColor;				// 前景颜色
            public NET_CFG_RGBA stuBackColor;				// 背景颜色
            public NET_CFG_RECT stuRect;					// 矩形区域
            public bool bShowEnable;				// 显示使能
        }

        // 画面颜色属性
        public struct NET_CFG_COLOR_INFO
        {
            public int nBrightness;				// 亮度(0-100)
            public int nContrast;					// 对比度(0-100)
            public int nSaturation;				// 饱和度(0-100)
            public int nHue;						// 色度(0-100)
            public int nGain;						// 增益(0-100)
            public bool bGainEn;					// 增益使能
        }
    }
}
