const std = @import("std");

pub fn solve(data: []const u8) !void {
    var it = std.mem.splitSequence(u8, data, "\n");
    var res: i32 = 0;
    while (it.next()) |s| {
        var n: i32 = try std.fmt.parseInt(i32, std.mem.trim(u8, s, " \r\t"), 10);
        n = @divFloor(n, 3) - 2;
        while (n > 0) : (n = @divFloor(n, 3) - 2) {
            res += n;
        }
    }
    try std.io.getStdOut().writer().print("{}\n", .{res});
}
