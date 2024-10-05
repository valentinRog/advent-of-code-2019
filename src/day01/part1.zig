const std = @import("std");
const add = @import("add");

pub fn solve(data: []const u8) !void {
    var it = std.mem.splitSequence(u8, data, "\n");
    var res: u32 = 0;
    while (it.next()) |s| {
        const n: u32 = try std.fmt.parseInt(u32, std.mem.trim(u8, s, " \t\r"), 10);
        res += n / 3 - 2;
    }
    try std.io.getStdOut().writer().print("{}\n", .{res});
}
