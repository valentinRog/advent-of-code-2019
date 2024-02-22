const std = @import("std");

pub fn main() !void {
    const stdin = std.io.getStdIn().reader();
    const alloc = std.heap.page_allocator;
    const ws = "\n\r ";
    const data = try stdin.readAllAlloc(alloc, 1 << 16);
    defer alloc.free(data);
    var it = std.mem.split(u8, std.mem.trim(u8, data, ws), "\n");
    var res: u32 = 0;
    while (it.next()) |s| {
        const n: u32 = try std.fmt.parseInt(u32, std.mem.trim(u8, s, ws), 10);
        res += n / 3 - 2;
    }
    try std.io.getStdOut().writer().print("{}\n", .{res});
}
