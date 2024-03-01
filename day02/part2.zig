const std = @import("std");

fn simulate(alloc: std.mem.Allocator, data: []u32, noun: u32, verb: u32) !u32 {
    const a = try alloc.dupe(u32, data);
    defer alloc.free(a);
    a[1] = noun;
    a[2] = verb;
    var i: usize = 0;
    while (true) : (i += 4) {
        switch (a[i]) {
            1 => a[a[i + 3]] = a[a[i + 1]] + a[a[i + 2]],
            2 => a[a[i + 3]] = a[a[i + 1]] * a[a[i + 2]],
            99 => break,
            else => unreachable,
        }
    }
    return a[0];
}

fn compute(alloc: std.mem.Allocator, l: std.ArrayList(u32)) !u32 {
    for (0..100) |i| {
        for (0..100) |ii| {
            if (try simulate(alloc, l.items, @intCast(i), @intCast(ii)) == 19690720) {
                return @intCast(100 * i + ii);
            }
        }
    }
    unreachable;
}

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
    const res = try compute(alloc, l);
    try std.io.getStdOut().writer().print("{}\n", .{res});
}
