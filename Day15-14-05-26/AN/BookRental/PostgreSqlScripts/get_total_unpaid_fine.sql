
CREATE OR REPLACE FUNCTION get_total_unpaid_fine(member_id integer)
RETURNS numeric AS $$
DECLARE
    v_total numeric;
BEGIN
    SELECT COALESCE(SUM(f."FineAmount"), 0)
    INTO v_total
    FROM "Fines" f
    JOIN "Borrowings" b ON f."BorrowingId" = b."BorrowingId"
    WHERE b."MemberId" = member_id AND f."PaidStatus" = 1;

    RETURN v_total;
END;
$$ LANGUAGE plpgsql;