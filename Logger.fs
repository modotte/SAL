// Copyright (c) 2021 Modotte
// This software is licensed under the GPL-3.0 license. For more details,
// please read the LICENSE file content.

namespace SAL

open Serilog

module Logger =
    let log = 
        LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("SAL-log.txt")
            .CreateLogger()
