import { App } from "./App"
import React from "react"
import { render } from "./test-utils"
import { screen } from "@testing-library/react"

test("renders learn react link", () => {
  render(<App />)
  const linkElement = screen.getByText(/reload/i)
  expect(linkElement).toBeInTheDocument()
})
