// Copyright (c) 2021 Modotte
// This software is licensed under the GPL-3.0 license. For more details,
// please read the LICENSE file content.

namespace SAL

module Logger =
    open Serilog
    let log = LoggerConfiguration().WriteTo.Console().CreateLogger()
