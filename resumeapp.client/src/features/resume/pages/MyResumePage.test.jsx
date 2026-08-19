import { fireEvent, render, screen, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import axiosClient from '../../../core/api/axiosClient'
import MyResumePage from './MyResumePage'

vi.mock('../../../core/api/axiosClient', () => ({
    default: {
        get: vi.fn(),
        put: vi.fn(),
    },
}))

// Updated: assertions use accessible field names and the current preview-link text.
describe('MyResumePage', () => {
    const resume = {
        id: 7,
        name: 'Future Ibeche',
        title: 'Full-Stack .NET Developer',
        email: 'future@example.com',
        url: 'https://example.com',
        stack: 'C#, ASP.NET Core, React, SQL Server',
        country: 'Nigeria',
        summary: 'I build secure and maintainable web applications.',
        updatedAt: '2026-08-09T12:00:00Z',
    }

    const renderMyResumePage = () =>
        render(
            <MemoryRouter>
                <MyResumePage />
            </MemoryRouter>,
        )

    beforeEach(() => {
        vi.clearAllMocks()
    })

    it('loads the signed-in user resume from the API', async () => {
        axiosClient.get.mockResolvedValue({ data: resume })

        renderMyResumePage()

        expect(screen.getByText('Loading your resume...')).toBeInTheDocument()

        await waitFor(() => {
            expect(axiosClient.get).toHaveBeenCalledWith('/Resume/my')
        })

        expect(await screen.findByText('Future Ibeche')).toBeInTheDocument()
    })

    it('displays the resume and future sections', async () => {
        axiosClient.get.mockResolvedValue({ data: resume })

        renderMyResumePage()

        expect(await screen.findByText('Current Resume')).toBeInTheDocument()
        expect(screen.getByText('Future Ibeche')).toBeInTheDocument()
        expect(screen.getByText('Full-Stack .NET Developer')).toBeInTheDocument()
        expect(screen.getByText(/future@example\.com/i)).toBeInTheDocument()
        expect(screen.getByText(/Nigeria/i)).toBeInTheDocument()
        expect(screen.getByText('Experience')).toBeInTheDocument()
        expect(screen.getByText('Skills')).toBeInTheDocument()
        expect(screen.getByText('Projects')).toBeInTheDocument()
        expect(screen.getByText('Certifications')).toBeInTheDocument()
        expect(screen.getByText('Education')).toBeInTheDocument()
        expect(screen.getByText('Referees')).toBeInTheDocument()
    })

    it('displays an error when the resume cannot be loaded', async () => {
        axiosClient.get.mockRejectedValue({
            response: {
                data: {
                    message: 'Unable to find your resume.',
                },
            },
        })

        renderMyResumePage()

        expect(await screen.findByRole('alert')).toHaveTextContent(
            'Unable to find your resume.',
        )
    })

    it('opens the edit form with the existing resume information', async () => {
        axiosClient.get.mockResolvedValue({ data: resume })

        renderMyResumePage()

        fireEvent.click(
            await screen.findByRole('button', { name: 'Edit Basic Info' }),
        )

        expect(
            screen.getByRole('heading', { name: 'Update your resume details' }),
        ).toBeInTheDocument()

        expect(
            screen.getByRole('textbox', { name: 'Name' }),
        ).toHaveValue('Future Ibeche')

        expect(
            screen.getByRole('textbox', { name: 'Title' }),
        ).toHaveValue('Full-Stack .NET Developer')

        expect(screen.getByLabelText('Email')).toHaveValue('future@example.com')
        expect(screen.getByLabelText('Country')).toHaveValue('Nigeria')
        expect(screen.getByLabelText('Summary')).toHaveValue(
            'I build secure and maintainable web applications.',
        )
    })

    it('closes the edit form when cancel is selected', async () => {
        axiosClient.get.mockResolvedValue({ data: resume })

        renderMyResumePage()

        fireEvent.click(
            await screen.findByRole('button', { name: 'Edit Basic Info' }),
        )

        fireEvent.click(screen.getByRole('button', { name: 'Cancel' }))

        expect(
            screen.queryByRole('heading', {
                name: 'Update your resume details',
            }),
        ).not.toBeInTheDocument()

        expect(screen.getByText('Current Resume')).toBeInTheDocument()
    })

    it('updates the resume through the API', async () => {
        axiosClient.get.mockResolvedValue({ data: resume })
        axiosClient.put.mockResolvedValue({ data: {} })

        renderMyResumePage()

        fireEvent.click(
            await screen.findByRole('button', { name: 'Edit Basic Info' }),
        )

        fireEvent.change(
            screen.getByRole('textbox', { name: 'Name' }),
            {
                target: { value: 'Future Wisdom Ibeche' },
            },
        )

        fireEvent.change(screen.getByLabelText('Summary'), {
            target: { value: 'Updated professional summary.' },
        })

        const form = screen
            .getByRole('button', { name: 'Save Changes' })
            .closest('form')

        fireEvent.submit(form)

        await waitFor(() => {
            expect(axiosClient.put).toHaveBeenCalledWith(
                '/Resume/7',
                expect.objectContaining({
                    name: 'Future Wisdom Ibeche',
                    summary: 'Updated professional summary.',
                }),
            )
        })

        expect(await screen.findByRole('status')).toHaveTextContent(
            'Resume updated successfully.',
        )
    })

    it('displays an error when the resume update fails', async () => {
        axiosClient.get.mockResolvedValue({ data: resume })
        axiosClient.put.mockRejectedValue({
            response: {
                data: {
                    message: 'Unable to update your resume.',
                },
            },
        })

        renderMyResumePage()

        fireEvent.click(
            await screen.findByRole('button', { name: 'Edit Basic Info' }),
        )

        const form = screen
            .getByRole('button', { name: 'Save Changes' })
            .closest('form')

        fireEvent.submit(form)

        expect(await screen.findByRole('alert')).toHaveTextContent(
            'Unable to update your resume.',
        )
    })

    it('provides a link to preview the resume', async () => {
        axiosClient.get.mockResolvedValue({ data: resume })

        renderMyResumePage()

        const previewLink = await screen.findByRole('link', {
            name: 'Preview Resume',
        })

        expect(previewLink).toHaveAttribute('href', '/my-resume/preview')
    })
})