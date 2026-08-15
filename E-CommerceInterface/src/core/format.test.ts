import { describe, expect, it } from "vitest";
import { formatCurrency, orderStatusLabel, paymentStatusLabel, productStatusLabel, refundStatusLabel, shortId } from "./format";

describe("display formatters", () => {
  it("formats backend values without inventing state", () => {
    expect(formatCurrency(12.5)).toBe("$12.50");
    expect(shortId("12345678-0000-0000-0000-000000000000")).toBe("12345678");
  });

  it("maps persisted enum values to readable labels", () => {
    expect(productStatusLabel(1)).toBe("Active");
    expect(orderStatusLabel(4)).toBe("Shipped");
    expect(paymentStatusLabel(2)).toBe("Paid");
    expect(paymentStatusLabel(3)).toBe("Failed");
    expect(refundStatusLabel(1)).toBe("Pending");
  });
});
