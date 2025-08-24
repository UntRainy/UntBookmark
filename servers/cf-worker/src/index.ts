function getServer(url: URL): string | null {
	if (url.searchParams.has("server")) {
		return url.searchParams.get("server");
	} else if (url.pathname.startsWith("/s/")) {
		return url.pathname.slice(3);
	} else {
		return null;
	}
}

export default {
	async fetch(request, env, ctx): Promise<Response> {
		if (request.method === "GET") {
			const url = new URL(request.url);
			const server = getServer(url);
			if (!server) {
				return new Response(null, { status: 400 });
			}
			const ip = await env.unt_bookmark.get(`connstr:${server}`);
			if (!ip) {
				return new Response(null, { status: 404 });
			}
			return new Response(ip);
		} else if (request.method === "POST") {
			const url = new URL(request.url);
			const server = getServer(url);
			if (!server) {
				return new Response(null, { status: 400 });
			}
			const allowedGslts = env.GSLTS.split(",");
			const gslt = request.headers.get("X-GSLT");
			if (!gslt || !allowedGslts.includes(gslt)) {
				return new Response(null, { status: 403 });
			}
			const body = (await request.text()).trim();
			if (!body) {
				return new Response("No connection address provided.", { status: 400 });
			}
			const ip = body.trim();
			await env.unt_bookmark.put(`connstr:${server}`, ip);
			return new Response(null, { status: 204 });
		}
		return new Response(null, { status: 405 });
	},
} satisfies ExportedHandler<Env>;
