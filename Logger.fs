namespace SAL

module Logger =
    open Serilog
    let log = LoggerConfiguration().WriteTo.Console().CreateLogger();