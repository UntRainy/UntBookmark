import { env, createExecutionContext, waitOnExecutionContext, SELF } from 'cloudflare:test';
import { describe, it, expect } from 'vitest';
import worker from '../src/index';

// For now, you'll need to do something like this to get a correctly-typed
// `Request` to pass to `worker.fetch()`.
const IncomingRequest = Request<unknown, IncomingRequestCfProperties>;

describe('UntBookmark worker', () => {
	it('responds with 400 for GET without server param', async () => {
		const request = new IncomingRequest('http://example.com');
		const ctx = createExecutionContext();
		const response = await worker.fetch(request, env, ctx);
		await waitOnExecutionContext(ctx);
		expect(response.status).toBe(400);
		await response.text(); // Ensure we consume the response body
	});

	it('responds with 404 for GET with non-existent server', async () => {
		const request = new IncomingRequest('http://example.com?server=nonexistent');
		const ctx = createExecutionContext();
		const response = await worker.fetch(request, env, ctx);
		await waitOnExecutionContext(ctx);
		expect(response.status).toBe(404);
		await response.text(); // Ensure we consume the response body
	});

	it('responds with 403 for POST with invalid GSLT', async () => {
		const request = new IncomingRequest('http://example.com?server=test', {
			method: 'POST',
			headers: { 'X-GSLT': 'invalid_gslt' },
			body: 'example.com:25565',
		});
		const ctx = createExecutionContext();
		const response = await worker.fetch(request, env, ctx);
		await waitOnExecutionContext(ctx);
		expect(response.status).toBe(403);
		await response.text(); // Ensure we consume the response body
	});

	it('responds with 400 for POST without body', async () => {
		const valid_gslt = env.GSLTS.split(',')[0];
		const request = new IncomingRequest('http://example.com?server=test', {
			method: 'POST',
			headers: { 'X-GSLT': valid_gslt },
		});
		const ctx = createExecutionContext();
		const response = await worker.fetch(request, env, ctx);
		await waitOnExecutionContext(ctx);
		expect(response.status).toBe(400);
		await response.text(); // Ensure we consume the response body
	});

	it('responds with 204 for POST with valid server and GSLT', async () => {
		const valid_gslt = env.GSLTS.split(',')[0];
		const request = new IncomingRequest('http://example.com?server=test', {
			method: 'POST',
			headers: { 'X-GSLT': valid_gslt },
			body: 'example.com:25565',
		});
		const ctx = createExecutionContext();
		const response = await worker.fetch(request, env, ctx);
		await waitOnExecutionContext(ctx);
		expect(response.status).toBe(204);
		await response.text(); // Ensure we consume the response body
	});

	it('responds with 200 for GET with valid server', async () => {
		const request = new IncomingRequest('http://example.com?server=test');
		const ctx = createExecutionContext();
		const response = await worker.fetch(request, env, ctx);
		await waitOnExecutionContext(ctx);
		expect(response.status).toBe(200);
		expect(await response.text()).toBe('example.com:25565');
	});

	it('responds with 405 for unsupported method', async () => {
		const request = new IncomingRequest('http://example.com', {
			method: 'PUT',
		});
		const ctx = createExecutionContext();
		const response = await worker.fetch(request, env, ctx);
		await waitOnExecutionContext(ctx);
		expect(response.status).toBe(405);
		await response.text(); // Ensure we consume the response body
	});
});
