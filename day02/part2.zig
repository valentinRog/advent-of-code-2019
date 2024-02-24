const std = @import("std");

fn simulate(l: std.ArrayList(u32), noun: u32, verb: u32) u32 {
    l.items[1] = noun;
    l.items[2] = verb;
    var i: usize = 0;
    while (true) : (i += 4) {
        switch (l.items[i]) {
            1 => l.items[l.items[i + 3]] = l.items[l.items[i + 1]] + l.items[l.items[i + 2]],
            2 => l.items[l.items[i + 3]] = l.items[l.items[i + 1]] * l.items[l.items[i + 2]],
            99 => break,
            else => unreachable,
        }
    }
    return l.items[0];
}

fn compute(l: std.ArrayList(u32)) !u32 {
    for (0..100) |i| {
        for (0..100) |ii| {
            const ll = try l.clone();
            defer ll.deinit();
            if (simulate(ll, @intCast(i), @intCast(ii)) == 19690720) {
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
    const res = try compute(l);
    try std.io.getStdOut().writer().print("{}\n", .{res});
}
