const std = @import("std");

pub fn main() !void {
    const stdin = std.io.getStdIn().reader();
    const alloc = std.heap.page_allocator;
    const ws = "\n\r ";
    const data = try stdin.readAllAlloc(alloc, 1 << 16);
    defer alloc.free(data);
    var it = std.mem.split(u8, std.mem.trim(u8, data, ws), "\n");
    var res: i32 = 0;
    while (it.next()) |s| {
        var n: i32 = try std.fmt.parseInt(i32, std.mem.trim(u8, s, ws), 10);
        n = @divFloor(n, 3) - 2;
        while (n > 0) : (n = @divFloor(n, 3) - 2) {
            res += n;
        }
    }
    try std.io.getStdOut().writer().print("{}\n", .{res});
}
