const std = @import("std");

fn isValid(n: u32) !bool {
    var buf: [6]u8 = undefined;
    _ = try std.fmt.bufPrint(&buf, "{}", .{n});
    var b = false;
    for (1..buf.len) |i| {
        if (buf[i] < buf[i - 1]) {
            return false;
        }
        if (buf[i] == buf[i - 1]) {
            b = true;
        }
    }
    return b;
}

pub fn main() !void {
    const stdin = std.io.getStdIn().reader();
    var gpa = std.heap.GeneralPurposeAllocator(.{}){};
    defer _ = gpa.deinit();
    const alloc = gpa.allocator();
    const raw = try stdin.readAllAlloc(alloc, 1 << 16);
    defer alloc.free(raw);
    var range: [2]u32 = undefined;
    {
        var it = std.mem.tokenizeScalar(u8, std.mem.trim(u8, raw, "\r\n "), '-');
        range[0] = try std.fmt.parseInt(u32, it.next().?, 10);
        range[1] = try std.fmt.parseInt(u32, it.next().?, 10);
    }
    var res: u32 = 0;
    for (range[0]..range[1] + 1) |n| {
        if (try isValid(@intCast(n))) {
            res += 1;
        }
    }
    try std.io.getStdOut().writer().print("{}\n", .{res});
}
