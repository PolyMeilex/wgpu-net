[[stage(vertex)]]
fn vs_main([[builtin(vertex_index)]] in_vertex_index: u32) -> [[builtin(position)]] vec4<f32> {
    let x = f32(i32(in_vertex_index) - 1);
    let y = f32(i32(in_vertex_index & 1u) * 2 - 1);
    return vec4<f32>(x, y, 0.0, 1.0);
}

[[stage(fragment)]]
fn fs_main([[builtin(position)]] pos: vec4<f32>) -> [[location(0)]] vec4<f32> {
    return vec4<f32>(pos.x / 100.0, pos.y / 900.0, 0.0, 1.0);
}