import {test} from 'node:test';
import assert from 'node:assert/strict';
import {tokenExpiresAt, clearSession} from '../src/context/session.ts';

const token = payload => `header.${Buffer.from(JSON.stringify(payload)).toString('base64url')}.signature`;

test('reads JWT expiry in milliseconds', () => {
  assert.equal(tokenExpiresAt(token({exp: 1800000000})), 1800000000000);
});

test('malformed tokens and missing or invalid expiry fail closed', () => {
  for (const value of ['broken', token({}), token({exp: 'tomorrow'})])
    assert.equal(tokenExpiresAt(value), 0);
});

test('expired tokens are distinguishable from valid sessions', () => {
  assert.ok(tokenExpiresAt(token({exp: Math.floor(Date.now() / 1000) - 1})) < Date.now());
});

test('clearing a session removes both stored credentials', () => {
  const removed = [];
  globalThis.localStorage = {removeItem: key => removed.push(key)};
  clearSession();
  assert.deepEqual(removed, ['token', 'user']);
  delete globalThis.localStorage;
});
