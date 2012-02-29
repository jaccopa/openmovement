/* 
 * Copyright (c) 2009-2012, Newcastle University, UK.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met: 
 * 1. Redistributions of source code must retain the above copyright notice, 
 *    this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright notice, 
 *    this list of conditions and the following disclaimer in the documentation 
 *    and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE 
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
 * POSSIBILITY OF SUCH DAMAGE. 
 */

// File system configuration.
// Derived from contents of FSconfig.h from Microchip Solutions.


#ifndef _FS_DEF_
#define _FS_DEF_

#include "HardwareProfile.h"

// Maximum number of files open at once (when dynamic memory allocation is not used)
#define FS_MAX_FILES_OPEN 	2

// Sector size (must be 512 bytes in this application)
#define MEDIA_SECTOR_SIZE 		512

// Enable file searching (FindFirst, FindNext, and FindPrev)
#define ALLOW_FILESEARCH

// Enable writing
#define ALLOW_WRITES

// Enable on-device formats (writes must be enabled)
#define ALLOW_FORMATS

// Directories enabled (writes must be enabled)
#define ALLOW_DIRS

// Allows the use of the FSfprintf function (writes must be enabled)
#define ALLOW_FSFPRINTF

// FAT32 support required
//#define SUPPORT_FAT32

// Timestamp configuration
#define USEREALTIMECLOCK        // Use the RTC (configured in  application code) to set the time
//#define USERDEFINEDCLOCK      // Timing variables updated manually using the SetClockVars function before create/close.
//#define INCREMENTTIMESTAMP    // Fake, incremented timestamps
#if !defined(USEREALTIMECLOCK) && !defined(USERDEFINEDCLOCK) && !defined(INCREMENTTIMESTAMP)
    #error "One of USEREALTIMECLOCK, USERDEFINEDCLOCK, or INCREMENTTIMESTAMP must be enabled"
#endif

// Define FS_DYNAMIC_MEM to use malloc for allocating FILE structure space.
//#define FS_DYNAMIC_MEM
#ifdef FS_DYNAMIC_MEM
    #define FS_malloc	malloc
    #define FS_free		free
#endif


// Define our MDD function implementations
#define MDD_MediaInitialize     MDD_FTL_MediaInitialize
#define MDD_MediaDetect         MDD_FTL_FSIO_MediaDetect
#define MDD_SectorRead          MDD_FTL_FSIO_SectorRead
#define MDD_SectorWrite         MDD_FTL_FSIO_SectorWrite
#define MDD_InitIO              MDD_FTL_InitIO
#define MDD_ShutdownMedia       MDD_FTL_ShutdownMedia
#define MDD_WriteProtectState   MDD_FTL_WriteProtectState
#define MDD_ReadSectorSize      MDD_FTL_ReadSectorSize
#define MDD_ReadCapacity        MDD_FTL_ReadCapacity

#define MDD_USB_MediaInitialize     MDD_FTL_MediaInitialize
#define MDD_USB_MediaDetect         MDD_FTL_USB_MediaDetect
#define MDD_USB_SectorRead          MDD_FTL_USB_SectorRead
#define MDD_USB_SectorWrite         MDD_FTL_USB_SectorWrite
#define MDD_USB_InitIO              MDD_FTL_InitIO
#define MDD_USB_ShutdownMedia       MDD_FTL_ShutdownMedia
#define MDD_USB_WriteProtectState   MDD_FTL_WriteProtectState
#define MDD_USB_ReadSectorSize      MDD_FTL_ReadSectorSize
#define MDD_USB_ReadCapacity        MDD_FTL_ReadCapacity


// 32kb cluster size
#define FORMAT_SECTORS_PER_CLUSTER 0x40
	

// Change the behaviour of FsFtl.h when included from this file
#define FSFTL_IN_FSCONFIG
#include "Ftl/FsFtl.h"
#undef FSFTL_IN_FSCONFIG


#endif
