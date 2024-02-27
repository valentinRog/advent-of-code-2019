const std = @import("std");

const P = struct {
    x: i32,
    y: i32,

    fn add(self: *const P, other: P) P {
        return P{ .x = self.x + other.x, .y = self.y + other.y };
    }

    fn man(self: *const P) !i32 {
        return (try std.math.absInt(self.x)) + (try std.math.absInt(self.y));
    }
};

fn parseCircuit(alloc: std.mem.Allocator, data: []const u8) !std.AutoHashMap(P, void) {
    var m = std.AutoHashMap(P, void).init(alloc);
    var p = P{ .x = 0, .y = 0 };
    var it = std.mem.tokenizeScalar(u8, data, ',');
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
            try m.put(p, void{});
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
    var circuits: [2]std.AutoHashMap(P, void) = undefined;
    defer circuits[0].deinit();
    defer circuits[1].deinit();
    {
        var it = std.mem.tokenizeScalar(u8, std.mem.trim(u8, raw, "\n\r "), '\n');
        circuits[0] = try parseCircuit(alloc, it.next().?);
        circuits[1] = try parseCircuit(alloc, it.next().?);
    }
    var res: i32 = std.math.maxInt(i32);
    {
        var it = circuits[0].keyIterator();
        while (it.next()) |p| {
            if (circuits[1].contains(p.*)) {
                res = @min(res, try p.man());
            }
        }
    }
    try std.io.getStdOut().writer().print("{}\n", .{res});
}
