const std = @import("std");

pub fn build(b: *std.Build) void {
    const day = b.option([]const u8, "day", "day").?;

    const exe = b.addExecutable(.{
        .name = "aoc",
        .root_source_file = b.path(b.fmt("src/{s}/main.zig", .{day})),
        .target = b.host,
    });

    exe.root_module.addImport("utils", b.createModule(.{
        .root_source_file = b.path("src/common/utils.zig"),
    }));

    b.installArtifact(exe);

    const run_exe = b.addRunArtifact(exe);

    const run_step = b.step("run", "run");
    run_step.dependOn(&run_exe.step);
}
