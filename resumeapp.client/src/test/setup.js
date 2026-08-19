import '@testing-library/jest-dom/vitest'
import { cleanup } from '@testing-library/react'
import { afterEach } from 'vitest'

// Cleans the rendered component after every test.
afterEach(() => {
    cleanup()
})