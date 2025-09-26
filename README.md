# Virtuelizacija Procesa - Electric Vehicle Charging Data Management System

## 🔋 Project Overview
A comprehensive WCF-based system for exchanging and analyzing electric vehicle charging data. The application processes telemetric data from charging sessions, including voltage, current, power,
and frequency measurements, enabling real-time monitoring and analytics of EV charging infrastructure.

## 🚗 System Architecture

```
Client Application ←→ WCF Service ←→ Server File System
     (CSV Reader)      (Data Processor)    (Storage & Analytics)
```

### Communication Flow
1. **StartSession**: Initialize charging session with vehicle ID
2. **PushSample**: Stream individual measurement records
3. **EndSession**: Finalize session and trigger analytics

## 📊 Data Structure

### DataContract Fields
- **Timestamp**: Measurement time
- **Voltage RMS**: Min/Avg/Max voltage values
- **Current RMS**: Min/Avg/Max current values  
- **Power Metrics**: Real/Reactive/Apparent power (Min/Avg/Max)
- **Frequency**: Min/Avg/Max frequency measurements
- **RowIndex**: Sequential record identifier
- **VehicleId**: Vehicle identification

## 🔧 Technical Implementation

### WCF Service Configuration
- **Binding**: `netTcpBinding` with streaming support
- **Message Size**: Enhanced `MaxReceivedMessageSize` for large datasets
- **Validation**: Real-time data validation with fault handling
- **Resource Management**: Proper `IDisposable` implementation

## 📈 Analytics Features

### Real-Time Monitoring
- **Voltage Spike Detection**: ΔV threshold monitoring
- **Current Anomaly Detection**: ΔI variance analysis
- **Power Factor Calculation**: Real Power / Apparent Power ratio
- **Warning System**: Automated alerts for parameter deviations

### Event System
- `OnTransferStarted`: Session initiation
- `OnSampleReceived`: Individual record processing
- `OnTransferCompleted`: Session completion
- `OnWarningRaised`: Anomaly detection alerts

## 🗂️ Data Source
Contains charging profiles for 12 different vehicle models with comprehensive telemetric data in CSV format.

## 🚀 Key Features

### Client Application
- **Multi-Vehicle Support**: 12 predefined vehicle models/folders
- **CSV Parsing**: Invariant culture formatting with error logging
- **Sequential Streaming**: Row-by-row data transmission
- **Resource Management**: Automatic cleanup with Dispose pattern

### Server Application  
- **Data Persistence**: Structured file storage system
- **Validation Engine**: Real-time data quality checks
- **Analytics Engine**: Statistical computation and trend analysis
- **Event-Driven Architecture**: Delegate-based notification system

### Network Communication
- **Streaming Protocol**: Efficient data transfer
- **Status Monitoring**: Real-time transfer progress indication
- **Fault Tolerance**: Graceful error handling and recovery
- **Connection Management**: Automatic resource cleanup

---

**⚡ Empowering Electric Vehicle Infrastructure Through Data Analytics**  
*Developed for FTN Process Virtualization Course*
