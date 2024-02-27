const std = @import("std");

const P = struct {
    x: i32,
    y: i32,

    fn add(self: *const P, other: P) P {
        return P{ .x = self.x + other.x, .y = self.y + other.y };
    }
};

fn parseCircuit(alloc: std.mem.Allocator, data: []const u8) !std.AutoHashMap(P, u32) {
    var m = std.AutoHashMap(P, u32).init(alloc);
    var p = P{ .x = 0, .y = 0 };
    var it = std.mem.tokenizeScalar(u8, data, ',');
    var step: u32 = 0;
    while (it.next()) |s| {
        const c = s[0];
        const n = try std.fmt.parseInt(usize, std.mem.trimRight(u8, s[1..], "\r"), 10);
        const d = switch (c) {
            'U' => P{ .x = 0, .y = 1 },
            'D' => P{ .x = 0, .y = -1 },
            'L' => P{ .x = -1, .y = 0 },
            'R' => P{ .x = 1, .y = 0 },
            else => unreachable,
        };
        for (0..n) |_| {
            p = p.add(d);
            step += 1;
            try m.put(p, step);
        }
    }
    return m;
}

pub fn main() !void {
    const stdin = std.io.getStdIn().reader();
    var gpa = std.heap.GeneralPurposeAllocator(.{}){};
    defer _ = gpa.deinit();
    const alloc = gpa.allocator();
    const raw = try stdin.readAllAlloc(alloc, 1 << 16);
    defer alloc.free(raw);
    var circuits: [2]std.AutoHashMap(P, u32) = undefined;
    defer circuits[0].deinit();
    defer circuits[1].deinit();
    {
        var it = std.mem.tokenizeScalar(u8, std.mem.trim(u8, raw, "\n\r "), '\n');
        circuits[0] = try parseCircuit(alloc, it.next().?);
        circuits[1] = try parseCircuit(alloc, it.next().?);
    }
    var res: u32 = std.math.maxInt(u32);
    {
        var it = circuits[0].keyIterator();
        while (it.next()) |p| {
            if (circuits[1].contains(p.*)) {
                res = @min(res, circuits[0].get(p.*).? + circuits[1].get(p.*).?);
            }
        }
    }
    try std.io.getStdOut().writer().print("{}\n", .{res});
}
