const std = @import("std");

pub fn main() !void {
    const stdin = std.io.getStdIn().reader();
    var gpa = std.heap.GeneralPurposeAllocator(.{}){};
    defer _ = gpa.deinit();
    const alloc = gpa.allocator();
    var l = std.ArrayList(u32).init(alloc);
    defer l.deinit();
    {
        const raw = try stdin.readAllAlloc(alloc, 1 << 16);
        defer alloc.free(raw);
        var it = std.mem.tokenizeScalar(u8, std.mem.trim(u8, raw, "\n\r "), ',');
        while (it.next()) |s| {
            const n = try std.fmt.parseInt(u32, s, 10);
            try l.append(n);
        }
    }
    {
        l.items[1] = 12;
        l.items[2] = 2;
        var i: usize = 0;
        while (true) : (i += 4) {
            switch (l.items[i]) {
                1 => l.items[l.items[i + 3]] = l.items[l.items[i + 1]] + l.items[l.items[i + 2]],
                2 => l.items[l.items[i + 3]] = l.items[l.items[i + 1]] * l.items[l.items[i + 2]],
                99 => break,
                else => unreachable,
            }
        }
    }
    try std.io.getStdOut().writer().print("{}\n", .{l.items[0]});
}
