using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace RecordManager
{
    // Schedule record configuration information
    public struct CFG_RECORD_INFO
    {
	    public int                 nChannelID;					// The channel number(Begins with 0)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
	    public ONEDAY_CFG_TIME_SECTION[]	stuTimeSections; // Time table
	    public int					nPreRecTime;				// Pre-record time.The value ranges from 0 to 300. This function is disable when the value is 0.
	    public bool				bRedundancyEn;				// Record redundancy enbale button
	    public int					nStreamType;				// 0-main stream,1-extra stream 1,2-extra stream 2,3-extra stream 3
	    public int					nProtocolVer;				// Protocol Version No., read only
	    // Capacity
        public bool                abHolidaySchedule;          // There are Holiday Configuration Information When it  is True, bHolidayEn,stuHolTimeSection is effective;
        public bool                bHolidayEn;                 // Holiday Video Enable TRUE:Enable,FALSE:Unable
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public CFG_TIME_SECTION[]    stuHolTimeSection;  // Holiday Video Schedule
    }

    // Period information
    public struct CFG_TIME_SECTION 
    {
	    public uint				dwRecordMask;						// Record subnet mask. The bit represents motion detect reocrd, alarm record, schedule record. Bit3~Bit15 is reserved. Bit 16 motion detect snapshot, Bit 17 alarm snapshot, Bit18 schedule snapshot
	    public int					nBeginHour;
	    public int					nBeginMin;
	    public int					nBeginSec;
	    public int					nEndHour;
	    public int					nEndMin;
	    public int					nEndSec;	
    }

    //one day period information
    public struct ONEDAY_CFG_TIME_SECTION
    {
        [MarshalAs(UnmanagedType.ByValArray,SizeConst = 6)]
        public CFG_TIME_SECTION[] stuTimeSection;
    }
}
