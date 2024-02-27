﻿namespace Meadow.Hcom;

// Messages sent from host to Meadow 
public enum RequestType : ushort
{
    HCOM_MDOW_REQUEST_UNDEFINED_REQUEST = 0x00 | ProtocolType.HCOM_PROTOCOL_HEADER_UNDEFINED_TYPE,

    // No longer supported
    // HCOM_MDOW_REQUEST_CREATE_ENTIRE_FLASH_FS  = 0x01 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_CHANGE_TRACE_LEVEL = 0x02 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_FORMAT_FLASH_FILE_SYS = 0x03 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_END_FILE_TRANSFER = 0x04 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_RESTART_PRIMARY_MCU = 0x05 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_VERIFY_ERASED_FLASH = 0x06 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    // No longer supported
    // HCOM_MDOW_REQUEST_PARTITION_FLASH_FS      = 0x07 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    // No longer supported
    // HCOM_MDOW_REQUEST_MOUNT_FLASH_FS          = 0x08 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    // No longer supported
    // HCOM_MDOW_REQUEST_INITIALIZE_FLASH_FS     = 0x09 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_BULK_FLASH_ERASE = 0x0a | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_ENTER_DFU_MODE = 0x0b | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_ENABLE_DISABLE_NSH = 0x0c | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_LIST_PARTITION_FILES = 0x0d | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_LIST_PART_FILES_AND_CRC = 0x0e | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_MONO_DISABLE = 0x0f | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_MONO_ENABLE = 0x10 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_MONO_RUN_STATE = 0x11 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_GET_DEVICE_INFORMATION = 0x12 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_PART_RENEW_FILE_SYS = 0x13 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_NO_TRACE_TO_HOST = 0x14 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_SEND_TRACE_TO_HOST = 0x15 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_END_ESP_FILE_TRANSFER = 0x16 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_READ_ESP_MAC_ADDRESS = 0x17 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_RESTART_ESP32 = 0x18 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_MONO_FLASH = 0x19 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_SEND_TRACE_TO_UART = 0x1a | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_NO_TRACE_TO_UART = 0x1b | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_SEND_PROFILER_TO_UART = 0x23 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_NO_PROFILER_TO_UART = 0x24 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    // >>> Breaking protocol change.
    // ToDo: This message is miscategorized should be ProtocolType.HCOM_PROTOCOL_HEADER_FILE_START_TYPE
    // like HCOM_MDOW_REQUEST_START_FILE_TRANSFER.
    HCOM_MDOW_REQUEST_MONO_UPDATE_RUNTIME = 0x1c | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_MONO_UPDATE_FILE_END = 0x1d | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_MONO_START_DBG_SESSION = 0x1e | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_GET_DEVICE_NAME = 0x1f | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,

    // >>> Breaking protocol change.
    // ToDo: This message is miscategorized should be ProtocolType.HCOM_PROTOCOL_HEADER_SIMPLE_TEXT_TYPE
    // since it is a header followed by text (the file name)
    HCOM_MDOW_REQUEST_GET_INITIAL_FILE_BYTES = 0x20 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_UPLOAD_START_DATA_SEND = 0x21 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_UPLOAD_ABORT_DATA_SEND = 0x22 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,

    // The file types have the optional data field defined for sending file information
    HCOM_MDOW_REQUEST_START_FILE_TRANSFER = 0x01 | ProtocolType.HCOM_PROTOCOL_HEADER_FILE_START_TYPE,
    HCOM_MDOW_REQUEST_DELETE_FILE_BY_NAME = 0x02 | ProtocolType.HCOM_PROTOCOL_HEADER_FILE_START_TYPE,
    HCOM_MDOW_REQUEST_START_ESP_FILE_TRANSFER = 0x03 | ProtocolType.HCOM_PROTOCOL_HEADER_FILE_START_TYPE,

    // These message are a header followed by text, one contains the texts length too
    HCOM_MDOW_REQUEST_UPLOAD_FILE_INIT = 0x01 | ProtocolType.HCOM_PROTOCOL_HEADER_SIMPLE_TEXT_TYPE,
    HCOM_MDOW_REQUEST_EXEC_DIAG_APP_CMD = 0x02 | ProtocolType.HCOM_PROTOCOL_HEADER_SIMPLE_TEXT_TYPE,
    HCOM_MDOW_REQUEST_RTC_SET_TIME_CMD = 0x03 | ProtocolType.HCOM_PROTOCOL_HEADER_SIMPLE_TEXT_TYPE,
    // ToDo HCOM_MDOW_REQUEST_RTC_READ_TIME_CMD doesn't send text, it's a header only message type
    HCOM_MDOW_REQUEST_RTC_READ_TIME_CMD = 0x04 | ProtocolType.HCOM_PROTOCOL_HEADER_SIMPLE_TEXT_TYPE,
    HCOM_MDOW_REQUEST_RTC_WAKEUP_TIME_CMD = 0x05 | ProtocolType.HCOM_PROTOCOL_HEADER_SIMPLE_TEXT_TYPE,
    HCOM_MDOW_REQUEST_LIST_FILES_SUBDIR = 0x06 | ProtocolType.HCOM_PROTOCOL_HEADER_SIMPLE_TEXT_TYPE,
    HCOM_MDOW_REQUEST_LIST_FILES_SUBDIR_CRC = 0x07 | ProtocolType.HCOM_PROTOCOL_HEADER_SIMPLE_TEXT_TYPE,


    // This is a simple type with binary data
    HCOM_MDOW_REQUEST_DEBUGGING_DEBUGGER_DATA = 0x01 | ProtocolType.HCOM_PROTOCOL_HEADER_SIMPLE_BINARY_TYPE,

    HCOM_MDOW_REQUEST_DEVELOPER = 0xf8 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_DEVELOPER_1 = 0xf0 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_DEVELOPER_2 = 0xf1 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_DEVELOPER_3 = 0xf2 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_DEVELOPER_4 = 0xf3 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,

    // Testing QSPI flash
    HCOM_MDOW_REQUEST_QSPI_FLASH_INIT = 0xf4 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_QSPI_FLASH_WRITE = 0xf5 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,
    HCOM_MDOW_REQUEST_QSPI_FLASH_READ = 0xf6 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,

    HCOM_MDOW_REQUEST_OTA_REGISTER_DEVICE = 0xf7 | ProtocolType.HCOM_PROTOCOL_HEADER_ONLY_TYPE,

    HCOM_HOST_REQUEST_UPLOADING_FILE_DATA = 0x03 | ProtocolType.HCOM_PROTOCOL_HEADER_SIMPLE_BINARY_TYPE,

}
